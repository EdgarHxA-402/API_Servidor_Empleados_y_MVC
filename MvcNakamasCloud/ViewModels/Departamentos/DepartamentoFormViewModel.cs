using System.ComponentModel.DataAnnotations;


namespace MvcNakamasCloud.ViewModels
{
    public class DepartamentoFormViewModel
    {
        public int IdDepartamento { get; set; }
        [Required]
        public string CodigoDepartamento { get; set; }
        [Required]
        public string NombreDepartamento { get; set; }
    }
}
