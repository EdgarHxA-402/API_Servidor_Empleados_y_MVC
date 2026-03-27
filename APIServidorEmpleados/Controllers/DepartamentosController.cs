using APIServidorEmpleados.Data;
using APIServidorEmpleados.DTOs;
using APIServidorEmpleados.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIServidorEmpleados.Controllers
{
    [Route("api/departamentos")]
    [ApiController]
    public class DepartamentosController : ControllerBase
    {
        private readonly NakamaCloudDbContext nakamaCloudDbContext;
        public DepartamentosController(NakamaCloudDbContext nakamaCloudDbContext)
        {
            this.nakamaCloudDbContext = nakamaCloudDbContext;
        }

        [HttpGet]
        public IActionResult GetDepartamentos()
        {
            var departamentos = nakamaCloudDbContext.Departamentos
            .ToList();

            return Ok(new { data = departamentos });
        }


        // GET: api/departamentos/5
        [HttpGet("{id}")]
        public IActionResult GetDepartamento(int id)
        {
            var departamento = nakamaCloudDbContext.Departamentos
                .FirstOrDefault(d => d.IdDepartamento == id);

            if (departamento == null)
                return NotFound(new { message = "Departamento no encontrado" });

            return Ok(new { data = departamento });
        }

        // POST: api/departamentos
        [HttpPost]
        public IActionResult CrearDepartamento([FromBody] CrearDepartamentoDto dto)
        {
            var departamento = new Departamento
            {
                CodigoDepartamento = dto.CodigoDepartamento,
                NombreDepartamento = dto.NombreDepartamento,
                Activo = true
            };

            nakamaCloudDbContext.Departamentos.Add(departamento);
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Departamento creado correctamente", data = departamento });
        }

        [HttpPut("{id}")]
        public IActionResult ModificarDepartamento(int id, [FromBody] ModificarDepartamentoDto dto)
        {
            var departamento = nakamaCloudDbContext.Departamentos
                .FirstOrDefault(d => d.IdDepartamento == id);

            if (departamento == null)
                return NotFound(new { message = "Departamento no encontrado" });

            departamento.NombreDepartamento = dto.NombreDepartamento;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Departamento modificado correctamente", data = departamento });
        }

        // PATCH: api/departamentos/5/desactivar
        [HttpPatch("{id}/desactivar")]
        public IActionResult DesactivarDepartamento(int id)
        {
            var departamento = nakamaCloudDbContext.Departamentos
                .FirstOrDefault(d => d.IdDepartamento == id);

            if (departamento == null)
                return NotFound(new { message = "Departamento no encontrado" });

            departamento.Activo = false;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Departamento desactivado correctamente" });
        }

        // PATCH: api/departamentos/5/activar
        [HttpPatch("{id}/activar")]
        public IActionResult ActivarDepartamento(int id)
        {
            var departamento = nakamaCloudDbContext.Departamentos
                .FirstOrDefault(d => d.IdDepartamento == id);

            if (departamento == null)
                return NotFound(new { message = "Departamento no encontrado" });

            departamento.Activo = true;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Departamento activado correctamente" });
        }

        // GET: api/departamentos/proximoCodigo
        [HttpGet("proximoCodigo")]
        public IActionResult GetProximoCodigo()
        {
            var ultimoDepartamento = nakamaCloudDbContext.Departamentos
                .OrderByDescending(e => e.IdDepartamento)
                .FirstOrDefault();

            string proximoCodigo;

            if (ultimoDepartamento == null)
            {
                proximoCodigo = "DEP-001";
            }
            else
            {
                var ultimoCodigo = ultimoDepartamento.CodigoDepartamento;
                var numero = int.Parse(ultimoCodigo.Split('-')[1]);
                proximoCodigo = $"DEP-{(numero + 1):D3}";
            }

            return Ok(new { proximoCodigo });
        }

    }
}