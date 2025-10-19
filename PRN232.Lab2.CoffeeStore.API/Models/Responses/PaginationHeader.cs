namespace PRN232.Lab2.CoffeeStore.API.Models.Responses
{
    public class PaginationHeader
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        
        public int TotalCount { get; set; }
        public int TotalCurrentResults { get; set; }
        public string PreviousPageLink { get; set; } = string.Empty;
        public string NextPageLink { get; set; } = string.Empty;
        public string FirstPageLink { get; set; } = string.Empty;
    }
}
