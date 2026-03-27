using System.ComponentModel.DataAnnotations;

namespace MvcNakamasCloud.ViewModels
{
    public class EmpleadoFormViewModel
    {
        public int IdEmpleado { get; set; }
        [Required]
        public string CodigoEmpleado { get; set; }
        public string? FotoRuta { get; set; }
        [Required]
        public string PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        [Required]
        public string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        [Required]
        public string FechaNacimiento { get; set; }
        [Required]
        public string FechaContratacion { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Asignacion de puesto
        [Required]
        public int IdDepartamento { get; set; }
        [Required]
        public int IdPuesto { get; set; }
        [Required]
        public decimal? Salario { get; set; }

        // Listas para dropdowns
        public List<DepartamentoViewModel>? Departamentos { get; set; }
        public List<PuestoViewModel>? Puestos { get; set; }
    }
}
