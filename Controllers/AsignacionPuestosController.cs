using APIServidorEmpleados.Data;
using APIServidorEmpleados.DTOs;
using APIServidorEmpleados.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIServidorEmpleados.Controllers
{
    [Route("api/asignacionPuestos")]
    [ApiController]
    public class AsignacionPuestosController : ControllerBase
    {
        private readonly NakamaCloudDbContext nakamaCloudDbContext;
        public AsignacionPuestosController(NakamaCloudDbContext nakamaCloudDbContext)
        {
            this.nakamaCloudDbContext = nakamaCloudDbContext;
        }
        [HttpGet]
        public IActionResult GetAsignacionPuestos()
        {
            var asignacionPuestos = nakamaCloudDbContext.AsignacionPuestos
               .Include(a => a.IdEmpleadoNavigation)
                .Include(a => a.IdPuestoNavigation)
            .ThenInclude(p => p.IdDepartamentoNavigation)
            .ToList();

            return Ok(new { data = asignacionPuestos });
        }

        // POST: api/asignacionPuestos
        [HttpPost]
        public IActionResult CrearAsignacion([FromBody] AsignacionPuestoDto dto)
        {
            var asignacion = new AsignacionPuesto
            {
                IdEmpleado = dto.IdEmpleado,
                IdPuesto = dto.IdPuesto,
                Salario = dto.Salario,
                FechaInicio = dto.FechaInicio,
                Estado = "Activo",
                Observaciones = dto.Observaciones
            };

            nakamaCloudDbContext.AsignacionPuestos.Add(asignacion);
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Asignación creada correctamente", data = asignacion });
        }

        // PUT: api/asignacionPuestos/porEmpleado/5
        [HttpPut("porEmpleado/{idEmpleado}")]
        public IActionResult ActualizarAsignacion(int idEmpleado, [FromBody] AsignacionPuestoDto dto)
        {
            // Cancelar asignacion activa
            var asignacionActiva = nakamaCloudDbContext.AsignacionPuestos
                .FirstOrDefault(a => a.IdEmpleado == idEmpleado && a.Estado == "Activo");

            if (asignacionActiva != null)
            {
                asignacionActiva.Estado = "Cancelado";
                asignacionActiva.FechaFin = DateOnly.FromDateTime(DateTime.Now);
            }

            // Crear nueva asignacion
            var nuevaAsignacion = new AsignacionPuesto
            {
                IdEmpleado = idEmpleado,
                IdPuesto = dto.IdPuesto,
                Salario = dto.Salario,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                Estado = "Activo",
                Observaciones = dto.Observaciones
            };

            nakamaCloudDbContext.AsignacionPuestos.Add(nuevaAsignacion);
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Asignación actualizada correctamente" });
        }
    }
}
