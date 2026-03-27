using System.ComponentModel.DataAnnotations;

namespace APIServidorEmpleados.DTOs
{
    public class AsignacionPuestoDto
    {
        [Required]
        public int IdEmpleado { get; set; }
        [Required]
        public int IdPuesto { get; set; }
        [Required]
        public decimal Salario { get; set; }
        [Required]
        public DateOnly FechaInicio { get; set; }
        public string? Observaciones { get; set; }
    }
}
