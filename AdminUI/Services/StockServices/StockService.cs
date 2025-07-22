using AdminUI.Models.Stock;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdminUI.Services
{
    public class StockService : IStockService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<StockService> _logger;
        private const string TokenKey = "auth_token";

        public StockService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<StockService> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private async Task<string> GetCurrentUserTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString(TokenKey) ?? string.Empty;
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await GetCurrentUserTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<StockTransactionsListResponseModel> GetTransactionsAsync(StockSearchFilterViewModel filter)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                // ✅ BƯỚC 1: Gọi count endpoint để lấy tổng số
                var countResult = await GetTransactionCountAsync(filter);
                var totalCount = countResult.Success ? countResult.TotalCount : 0;

                _logger.LogInformation($"📊 Total transaction count: {totalCount}");

                // ✅ BƯỚC 2: Build OData query để lấy data với pagination
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    queryParams.Add($"$filter=contains(tolower(note),'{filter.SearchTerm.ToLower()}')");
                }

                if (!string.IsNullOrEmpty(filter.TransactionType))
                {
                    var typeFilter = $"transactionType eq '{filter.TransactionType}'";
                    if (queryParams.Any(q => q.StartsWith("$filter")))
                    {
                        queryParams[0] += $" and {typeFilter}";
                    }
                    else
                    {
                        queryParams.Add($"$filter={typeFilter}");
                    }
                }

                if (!string.IsNullOrEmpty(filter.Status))
                {
                    var statusFilter = $"status eq '{filter.Status}'";
                    if (queryParams.Any(q => q.StartsWith("$filter")))
                    {
                        queryParams[0] += $" and {statusFilter}";
                    }
                    else
                    {
                        queryParams.Add($"$filter={statusFilter}");
                    }
                }

                if (filter.CreatedBy.HasValue)
                {
                    var creatorFilter = $"createdBy eq {filter.CreatedBy.Value}";
                    if (queryParams.Any(q => q.StartsWith("$filter")))
                    {
                        queryParams[0] += $" and {creatorFilter}";
                    }
                    else
                    {
                        queryParams.Add($"$filter={creatorFilter}");
                    }
                }

                if (filter.FromDate.HasValue)
                {
                    var fromDateFilter = $"transactionDate ge {filter.FromDate.Value:yyyy-MM-ddTHH:mm:ssZ}";
                    if (queryParams.Any(q => q.StartsWith("$filter")))
                    {
                        queryParams[0] += $" and {fromDateFilter}";
                    }
                    else
                    {
                        queryParams.Add($"$filter={fromDateFilter}");
                    }
                }

                if (filter.ToDate.HasValue)
                {
                    var toDateFilter = $"transactionDate le {filter.ToDate.Value:yyyy-MM-ddTHH:mm:ssZ}";
                    if (queryParams.Any(q => q.StartsWith("$filter")))
                    {
                        queryParams[0] += $" and {toDateFilter}";
                    }
                    else
                    {
                        queryParams.Add($"$filter={toDateFilter}");
                    }
                }

                queryParams.Add($"$skip={((filter.Page - 1) * filter.PageSize)}");
                queryParams.Add($"$top={filter.PageSize}");
                queryParams.Add($"$orderby={filter.SortBy} {filter.SortOrder}");

                var queryString = string.Join("&", queryParams);
                var url = $"/gateway/stock/odata/transactions?{queryString}";

                _logger.LogInformation($"Calling Stock OData URL: {url}");

                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Stock OData Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // Parse OData response
                        if (responseContent.TrimStart().StartsWith("{") && responseContent.Contains("@odata"))
                        {
                            var odataResponse = JsonSerializer.Deserialize<StandardODataResponse<StockTransactionViewModel>>(responseContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            return new StockTransactionsListResponseModel
                            {
                                Success = true,
                                Data = odataResponse.Value ?? new List<StockTransactionViewModel>(),
                                TotalCount = totalCount // ✅ Sử dụng count từ endpoint riêng
                            };
                        }
                        else
                        {
                            var transactionsList = JsonSerializer.Deserialize<List<StockTransactionViewModel>>(responseContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            return new StockTransactionsListResponseModel
                            {
                                Success = true,
                                Data = transactionsList ?? new List<StockTransactionViewModel>(),
                                TotalCount = totalCount // ✅ Sử dụng count từ endpoint riêng
                            };
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, $"JSON Deserialization error. Response content: {responseContent}");
                        return new StockTransactionsListResponseModel
                        {
                            Success = false,
                            Message = $"Failed to parse response: {jsonEx.Message}"
                        };
                    }
                }
                else
                {
                    return new StockTransactionsListResponseModel
                    {
                        Success = false,
                        Message = $"Failed to get transactions: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock transactions");
                return new StockTransactionsListResponseModel
                {
                    Success = false,
                    Message = "An error occurred while retrieving transactions."
                };
            }
        }

        // ✅ THÊM: Method để gọi count endpoint
        private async Task<StockTransactionsListResponseModel> GetTransactionCountAsync(StockSearchFilterViewModel filter)
        {
            try
            {
                var countParams = new List<string>();

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    countParams.Add($"searchTerm={Uri.EscapeDataString(filter.SearchTerm)}");
                }

                if (!string.IsNullOrEmpty(filter.TransactionType))
                {
                    countParams.Add($"transactionType={filter.TransactionType}");
                }

                if (!string.IsNullOrEmpty(filter.Status))
                {
                    countParams.Add($"status={filter.Status}");
                }

                if (filter.CreatedBy.HasValue)
                {
                    countParams.Add($"createdBy={filter.CreatedBy.Value}");
                }

                if (filter.FromDate.HasValue)
                {
                    countParams.Add($"fromDate={filter.FromDate.Value:yyyy-MM-ddTHH:mm:ssZ}");
                }

                if (filter.ToDate.HasValue)
                {
                    countParams.Add($"toDate={filter.ToDate.Value:yyyy-MM-ddTHH:mm:ssZ}");
                }

                var countQueryString = string.Join("&", countParams);
                var countUrl = $"/gateway/stock/count?{countQueryString}";

                _logger.LogInformation($"Calling Stock Count URL: {countUrl}");

                var countResponse = await _httpClient.GetAsync(countUrl);

                if (countResponse.IsSuccessStatusCode)
                {
                    var countContent = await countResponse.Content.ReadAsStringAsync();
                    if (int.TryParse(countContent, out var count))
                    {
                        return new StockTransactionsListResponseModel
                        {
                            Success = true,
                            TotalCount = count
                        };
                    }
                }

                _logger.LogWarning($"Failed to get count: {countResponse.StatusCode}");
                return new StockTransactionsListResponseModel
                {
                    Success = false,
                    TotalCount = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction count");
                return new StockTransactionsListResponseModel
                {
                    Success = false,
                    TotalCount = 0
                };
            }
        }
        public async Task<StockTransactionResponseModel> GetTransactionByIdAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.GetAsync($"/gateway/stock/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    StockTransactionViewModel transaction = null;

                    if (apiResponse.TryGetProperty("data", out var dataElement))
                    {
                        transaction = JsonSerializer.Deserialize<StockTransactionViewModel>(dataElement.GetRawText(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    else
                    {
                        transaction = JsonSerializer.Deserialize<StockTransactionViewModel>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }

                    // ✅ SỬA: Đặt BookCode = BookISBN nếu BookCode trống
                    if (transaction?.Details != null)
                    {
                        foreach (var detail in transaction.Details)
                        {
                            if (string.IsNullOrEmpty(detail.BookCode) && !string.IsNullOrEmpty(detail.BookISBN))
                            {
                                detail.BookCode = detail.BookISBN;
                            }
                        }
                    }

                    return new StockTransactionResponseModel
                    {
                        Success = true,
                        Data = transaction
                    };
                }
                else
                {
                    return new StockTransactionResponseModel
                    {
                        Success = false,
                        Message = $"Failed to get transaction: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction by id");
                return new StockTransactionResponseModel
                {
                    Success = false,
                    Message = "An error occurred while retrieving transaction."
                };
            }
        }

        public async Task<StockTransactionResponseModel> CreateTransactionAsync(CreateStockTransactionViewModel model)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var createData = new
                {
                    transactionType = model.TransactionType,
                    note = model.Note,
                    details = model.Details.Select(d => new
                    {
                        bookID = d.BookID,
                        quantity = d.Quantity,
                        unitPrice = d.UnitPrice,
                        note = d.Note
                    }).ToList()
                };

                var jsonContent = JsonSerializer.Serialize(createData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/gateway/stock", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var transactionResponse = JsonSerializer.Deserialize<StockTransactionResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return transactionResponse ?? new StockTransactionResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    return new StockTransactionResponseModel
                    {
                        Success = false,
                        Message = $"Failed to create transaction: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                return new StockTransactionResponseModel
                {
                    Success = false,
                    Message = "An error occurred while creating transaction."
                };
            }
        }

        public async Task<StockTransactionResponseModel> UpdateTransactionStatusAsync(int id, UpdateTransactionStatusViewModel model)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var updateData = new
                {
                    status = model.Status,
                    note = model.Note
                };

                var jsonContent = JsonSerializer.Serialize(updateData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/gateway/stock/{id}/status", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var transactionResponse = JsonSerializer.Deserialize<StockTransactionResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return transactionResponse ?? new StockTransactionResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    return new StockTransactionResponseModel
                    {
                        Success = false,
                        Message = $"Failed to update transaction: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction status");
                return new StockTransactionResponseModel
                {
                    Success = false,
                    Message = "An error occurred while updating transaction status."
                };
            }
        }

        public async Task<StockTransactionResponseModel> DeleteTransactionAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.DeleteAsync($"/gateway/stock/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new StockTransactionResponseModel
                    {
                        Success = true,
                        Message = "Transaction deleted successfully"
                    };
                }
                else
                {
                    return new StockTransactionResponseModel
                    {
                        Success = false,
                        Message = $"Failed to delete transaction: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction");
                return new StockTransactionResponseModel
                {
                    Success = false,
                    Message = "An error occurred while deleting transaction."
                };
            }
        }
    }

    public class StandardODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; }

        [JsonPropertyName("@odata.count")]
        public int? OdataCount { get; set; }

        [JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; }

        [JsonPropertyName("@odata.nextLink")]
        public string OdataNextLink { get; set; }
    }
}