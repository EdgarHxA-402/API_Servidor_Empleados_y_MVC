using System.ComponentModel.DataAnnotations;

namespace APIServidorEmpleados.DTOs
{
    public class ModificarEmpleadoDto
    {
        [Required]
        public string PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        [Required]
        public string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        [Required]
        public DateOnly FechaNacimiento { get; set; }
        [Required]
        public DateOnly FechaContratacion { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
