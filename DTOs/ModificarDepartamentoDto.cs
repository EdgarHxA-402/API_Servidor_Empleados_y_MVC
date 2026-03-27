using System.ComponentModel.DataAnnotations;

namespace APIServidorEmpleados.DTOs
{
    public class ModificarDepartamentoDto
    {
        [Required]
        public string NombreDepartamento { get; set; }
    }
}
