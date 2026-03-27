namespace MvcNakamasCloud.ViewModels
{
    public class PaginationResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int Pages { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
