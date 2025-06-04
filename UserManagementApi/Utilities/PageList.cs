namespace UserManagementApi.Utilities
{
    public class PageList<T> where T : class
    {
        public List<T> Items { get; }
        public int TotalCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNext => PageNumber < TotalPages;
        public bool HasPrevious => PageNumber > 1;

        public PageList(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
