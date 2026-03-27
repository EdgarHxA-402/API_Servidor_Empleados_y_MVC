using MvcNakamasCloud.ViewModels;

namespace MvcNakamasCloud.ViewModels
{
    public class EmpleadoListViewModel
    {
        public IEnumerable<EmpleadoViewModel> Empleados { get; set; }
        public PaginationInfo Pagination { get; set; }
        public string? FiltroNombre { get; set; }
        public string? FiltroApellido { get; set; }
        public string? FiltroFechaContratacion { get; set; }
    }
}