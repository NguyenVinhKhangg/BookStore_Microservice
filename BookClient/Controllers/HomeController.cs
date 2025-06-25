using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BookClient.Models;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly HttpClient _client;
    private readonly string bookApiUrl = "https://localhost:7201/api/Book";
    private readonly string categoryApiUrl = "https://localhost:7261/api/Categories";
    private readonly string authorApiUrl = "https://localhost:7201/api/Authors";
    private readonly string publisherApiUrl = "https://localhost:7201/api/Publishers";

    public HomeController()
    {
        _client = new HttpClient();
        // Nếu API yêu cầu xác thực, thêm token
        // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your_jwt_token_here");
    }

    public async Task<IActionResult> Index(string categoryIds)
    {
        ViewBag.SelectedCategoryIds = categoryIds;
        Console.WriteLine($"Received categoryIds: {categoryIds}");

        if (!await CheckApiConnectionAsync())
        {
            ViewBag.ErrorMessage = "Không thể kết nối tới API. Vui lòng kiểm tra URL hoặc trạng thái server.";
            ViewBag.Categories = new List<Category>();
            ViewBag.AllBooks = new List<Book>();
            return View(new List<Book>());
        }

        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        List<Book> allBooks;
        try
        {
            var bookResponse = await _client.GetAsync(bookApiUrl);
            var bookJson = await bookResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Book API JSON: {bookJson}");
            allBooks = await _client.GetFromJsonAsync<List<Book>>(bookApiUrl, opts) ?? new List<Book>();
            Console.WriteLine($"All books retrieved: {allBooks.Count}");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Lỗi khi lấy dữ liệu sách: " + ex.Message;
            allBooks = new List<Book>();
        }

        List<Category> categories;
        try
        {
            var categoryResponse = await _client.GetAsync(categoryApiUrl);
            var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Category API JSON: {categoryJson}");
            categories = await _client.GetFromJsonAsync<List<Category>>(categoryApiUrl, opts) ?? new List<Category>();
            if (!categories.Any())
            {
                ViewBag.ErrorMessage = "Không tìm thấy danh mục nào từ API.";
            }
            Console.WriteLine($"Categories retrieved: {categories.Count}");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Lỗi khi lấy danh mục: " + ex.Message;
            categories = new List<Category>();
        }

        ViewBag.Categories = categories;
        ViewBag.AllBooks = allBooks;

        var categoryDict = categories.ToDictionary(c => c.CategoryID, c => c.Name);
        foreach (var book in allBooks)
        {
            book.CategoryName = book.CategoryID != 0 && categoryDict.ContainsKey(book.CategoryID)
                ? categoryDict[book.CategoryID]
                : "Không xác định";
        }

        List<Book> filteredBooks = allBooks.ToList();
        if (!string.IsNullOrEmpty(categoryIds))
        {
            var selectedCategoryIds = categoryIds.Split(',').Select(id => int.TryParse(id, out var parsedId) ? parsedId : 0)
                .Where(id => id != 0).ToList();
            if (selectedCategoryIds.Any())
            {
                filteredBooks = filteredBooks.Where(b => selectedCategoryIds.Contains(b.CategoryID)).ToList();
                Console.WriteLine($"Filtered books for categoryIds: {string.Join(",", selectedCategoryIds)}, Count: {filteredBooks.Count}");
            }
        }

        return View(filteredBooks);
    }

    private async Task<bool> CheckApiConnectionAsync()
    {
        try
        {
            var response = await _client.GetAsync(bookApiUrl);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        Book book;
        try
        {
            book = await _client.GetFromJsonAsync<Book>($"{bookApiUrl}/{id}/detailBook", opts);
            if (book != null && book.CategoryID != 0)
            {
                try
                {
                    var category = await _client.GetFromJsonAsync<Category>($"{categoryApiUrl}/{book.CategoryID}", opts);
                    book.CategoryName = category?.Name ?? "Không xác định";
                }
                catch
                {
                    book.CategoryName = "Không xác định";
                }
            }
            else
            {
                book.CategoryName = "Không xác định";
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Lỗi khi lấy chi tiết sách: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }

        if (book == null)
            return NotFound();

        return View(book);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var categories = await _client.GetFromJsonAsync<List<Category>>(categoryApiUrl, opts) ?? new List<Category>();
        ViewBag.Categories = categories;
        return View(new Book());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Book model, List<IFormFile> Images, string[] SelectedCategoryIds, string[] ImageUrls)
    {
       

        //if (!ModelState.IsValid)
        //{
        //    var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //    List<Category>? categories = null;
        //    try
        //    {
        //        categories = await _client.GetFromJsonAsync<List<Category>>(categoryApiUrl, opts) ?? new List<Category>();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrorMessage = "Lỗi khi lấy danh mục: " + ex.Message;
        //        categories = new List<Category>();
        //    }
        //    ViewBag.Categories = categories;
        //    ViewBag.SelectedCategoryIds = string.Join(",", SelectedCategoryIds ?? Array.Empty<string>());
        //    return View(model);
        //}

        var bookCreateDto = new Book
        {
            Title = model.Title,
            AuthorName = model.AuthorName,
            ISBN = model.ISBN,
            Price = model.Price,
            Stock = model.Stock,
            PublisherName = model.PublisherName,
            CategoryID = SelectedCategoryIds?.Length > 0 ? int.Parse(SelectedCategoryIds[0]) : 0
        };

        var response = await _client.PostAsJsonAsync(bookApiUrl, bookCreateDto);
        if (!response.IsSuccessStatusCode)
        {
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Category>? categories;
            try
            {
                categories = await _client.GetFromJsonAsync<List<Category>>(categoryApiUrl, opts) ?? new List<Category>();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi lấy danh mục: " + ex.Message;
                categories = new List<Category>();
            }
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryIds = string.Join(",", SelectedCategoryIds ?? Array.Empty<string>());
            ModelState.AddModelError("", "Không tạo được sách mới. Lỗi từ API.");
            return View(model);
        }

        var createdBook = await response.Content.ReadFromJsonAsync<Book>();
        if (createdBook == null)
        {
            ViewBag.ErrorMessage = "Tạo sách thất bại. Không nhận được dữ liệu từ API.";
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Category>? categories;
            try
            {
                categories = await _client.GetFromJsonAsync<List<Category>>(categoryApiUrl, opts) ?? new List<Category>();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage += " Lỗi khi lấy danh mục: " + ex.Message;
                categories = new List<Category>();
            }
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryIds = string.Join(",", SelectedCategoryIds ?? Array.Empty<string>());
            return View(model);
        }

        var createdBookId = createdBook.BookID;

        if (SelectedCategoryIds != null && SelectedCategoryIds.Length > 0)
        {
            var categoryApi = "https://localhost:7201/api/BookCategories";
            foreach (var catId in SelectedCategoryIds)
            {
                var categoryDto = new { BookID = createdBookId, CategoryID = int.Parse(catId) };
                await _client.PostAsJsonAsync(categoryApi, categoryDto);
            }
        }

        bool allImagesUploaded = true;
        if (Images != null)
        {
            foreach (var file in Images)
            {
                if (file?.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        var imgBytes = ms.ToArray();
                        var imgContent = new MultipartFormDataContent();
                        imgContent.Add(new ByteArrayContent(imgBytes), "ImageFile", file.FileName);
                        imgContent.Add(new StringContent(createdBookId.ToString()), "BookID");

                        var imgApiUrl = "https://localhost:7201/api/BooksImg";
                        var imgRes = await _client.PostAsync(imgApiUrl, imgContent);
                        if (!imgRes.IsSuccessStatusCode)
                        {
                            allImagesUploaded = false;
                        }
                    }
                }
            }
        }

        if (ImageUrls != null)
        {
            var imgApiUrl = "https://localhost:7201/api/BooksImgUrl";
            foreach (var url in ImageUrls.Where(u => !string.IsNullOrEmpty(u)))
            {
                var imgDto = new { BookID = createdBookId, ImageUrl = url };
                var imgRes = await _client.PostAsJsonAsync(imgApiUrl, imgDto);
                if (!imgRes.IsSuccessStatusCode)
                {
                    allImagesUploaded = false;
                }
            }
        }

        TempData["SuccessMessage"] = allImagesUploaded ? "Tạo sách thành công!" : "Tạo sách thành công nhưng có lỗi khi tải ảnh.";
        return RedirectToAction("Index");
    }
}