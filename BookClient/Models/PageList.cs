using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BookClient.Models
{
    public class PageList<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonIgnore]
        public int TotalPages
            => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}