namespace ServicesApp.Domain.DTOs
{
    public class PagedResultDto<T> where T : class
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
