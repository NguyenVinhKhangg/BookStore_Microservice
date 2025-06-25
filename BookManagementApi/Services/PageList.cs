using BookManagementApi.Models;

namespace BookManagementApi.Services
{
    public class PageList<T>
    {
        private List<Book> items;
        private int totalCount;
        private int pageNumber;
        private int pageSize;

        public PageList(List<Book> items, int totalCount, int pageNumber, int pageSize)
        {
            this.items = items;
            this.totalCount = totalCount;
            this.pageNumber = pageNumber;
            this.pageSize = pageSize;
        }
    }
}