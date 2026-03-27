using System.ComponentModel.DataAnnotations;

namespace APIServidorEmpleados.DTOs
{
    public class ModificarPuestoDto
    {
        [Required]
        public string NombrePuesto { get; set; }
        public string? DescripcionPuesto { get; set; }
        [Required]
        public int IdDepartamento { get; set; }
    }
}
