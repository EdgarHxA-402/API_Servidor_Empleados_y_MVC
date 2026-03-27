namespace MvcNakamasCloud.ViewModels
{
    public class PaginationViewModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int Pages { get; set; }
        public string? FiltroNombre { get; set; }
        public string? FiltroApellido { get; set; }
        public string? FiltroFechaContratacion { get; set; }
    }
}
