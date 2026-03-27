using System.ComponentModel.DataAnnotations;

namespace APIServidorEmpleados.DTOs
{
    public class CrearDepartamentoDto
    {
        [Required]
        public string CodigoDepartamento { get; set; }
        [Required]
        public string NombreDepartamento { get; set; }
    }
}
