using BookManagementApi.DTOs;
using BookManagementApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookManagementApi.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Book book);
        Task<bool> HideAsync(int id);
        Task<Book> AddAsync(Book book);

        Task<bool> UnhideAsync(int id);

    }

}
