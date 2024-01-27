namespace ServicesApp.Domain.DTOs
{
    public class PagedSearchInputsDto
    {
        public string SearchKey { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
