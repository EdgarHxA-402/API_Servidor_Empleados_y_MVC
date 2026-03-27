using System.ComponentModel.DataAnnotations;

namespace MvcNakamasCloud.ViewModels
{
    public class PuestoFormViewModel
    {
        public int IdPuesto { get; set; }
        [Required]
        public string CodigoPuesto { get; set; }
        [Required]
        public string NombrePuesto { get; set; }
        public string? DescripcionPuesto { get; set; }
        [Required]
        public int IdDepartamento { get; set; }
        public List<DepartamentoViewModel>? Departamentos { get; set; }
    }
}
