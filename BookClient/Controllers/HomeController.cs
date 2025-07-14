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

    private readonly string bookApiUrl = "https://localhost:7000/gateway/books";
    private readonly string categoryApiUrl = "https://localhost:7000/gateway/categories";
    // private readonly string authorApiUrl = "https://localhost:7000/gateway/authors";
    // private readonly string publisherApiUrl = "https://localhost:7000/gateway/publishers";

    public HomeController()
    {
        _client = new HttpClient();

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
    public async Task<IActionResult> Create(Book model, string[] SelectedCategoryIds, string ImageUrl)
    {
        var bookCreateDto = new Book
        {
            Title = model.Title,
            Description = model.Description,
            ISBN = model.ISBN,
            Price = model.Price,
            Discount = model.Discount,
            Stock = model.Stock,
            CategoryID = SelectedCategoryIds?.Length > 0 ? int.Parse(SelectedCategoryIds[0]) : 0,
            AuthorID = model.AuthorID,
            AuthorName = model.AuthorName,
            PublisherID = model.PublisherID,
            PublisherName = model.PublisherName,
            ImageUrl = model.ImageUrl,
            IsActive = false
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

        TempData["SuccessMessage"] = "Tạo sách thành công!";
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Lấy chi tiết sách qua Gateway
        var book = await _client.GetFromJsonAsync<Book>(
            $"{bookApiUrl}/{id}/detailBook", opts);
        if (book == null) return NotFound();

        // Lấy danh sách categories qua Gateway
        ViewBag.Categories = await _client.GetFromJsonAsync<List<Category>>(
            categoryApiUrl, opts) ?? new List<Category>();
        return View(book);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    int id,
    Book model,
    string[] SelectedCategoryIds)
    {
        if (id != model.BookID)
            return BadRequest();

        // Kiểm tra xem có category nào được chọn không
        int categoryId = model.CategoryID; // Giữ nguyên ID cũ làm mặc định

        if (SelectedCategoryIds != null && SelectedCategoryIds.Length > 0 &&
            !string.IsNullOrEmpty(SelectedCategoryIds[0]) &&
            int.TryParse(SelectedCategoryIds[0], out int newCategoryId))
        {
            categoryId = newCategoryId; // Sử dụng ID mới nếu có chọn
        }

        // Chuẩn bị DTO để gửi lên API
        var dto = new Book
        {
            Title = model.Title,
            Description = model.Description,
            ISBN = model.ISBN,
            Price = model.Price,
            Discount = model.Discount,
            Stock = model.Stock,
            CategoryID = categoryId, // Sử dụng categoryId đã xử lý
            AuthorID = model.AuthorID,
            AuthorName = model.AuthorName,
            PublisherID = model.PublisherID,
            PublisherName = model.PublisherName,
            ImageUrl = model.ImageUrl,
            IsActive = model.IsActive
        };

        // Gọi PUT qua Gateway (đúng RESTful và chuẩn)
        var response = await _client.PutAsJsonAsync(
            $"{bookApiUrl}/{id}/update", dto);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Cập nhật thất bại, vui lòng thử lại.");
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            ViewBag.Categories = await _client.GetFromJsonAsync<List<Category>>(
                categoryApiUrl, opts) ?? new List<Category>();
            return View(model);
        }

        TempData["SuccessMessage"] = "Cập nhật sách thành công!";
        return RedirectToAction(nameof(Index));
    }


    // Thanh cuộn 
    public async Task<IActionResult> GetAllBooks()
    {
        try
        {
            // Gọi API Gateway để lấy tất cả sách
            var books = await _client.GetFromJsonAsync<List<Book>>(bookApiUrl);

            // Kiểm tra nếu không có dữ liệu
            if (books == null || !books.Any())
            {
                ViewBag.ErrorMessage = "Không có sách nào trong cơ sở dữ liệu.";
                return View("Error");
            }

            // Trả về danh sách sách
            return View("Index", books);

        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Lỗi khi lấy dữ liệu sách: " + ex.Message;
            return View("Error");
        }
    }
}