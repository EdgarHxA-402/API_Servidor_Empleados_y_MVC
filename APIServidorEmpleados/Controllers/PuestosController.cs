using APIServidorEmpleados.Data;
using APIServidorEmpleados.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIServidorEmpleados.DTOs;

namespace APIServidorEmpleados.Controllers
{
    [Route("api/puestos")]
    [ApiController]
    public class PuestosController : ControllerBase
    {
        private readonly NakamaCloudDbContext nakamaCloudDbContext;

        public PuestosController(NakamaCloudDbContext nakamaCloudDbContext)
        {
            this.nakamaCloudDbContext = nakamaCloudDbContext;
        }

        // GET: api/puestos
        [HttpGet]
        public IActionResult GetPuestos()
        {
            var puestos = nakamaCloudDbContext.Puestos
                .Include(p => p.IdDepartamentoNavigation)
                .ToList();

            return Ok(new { data = puestos });
        }

        // GET: api/puestos/5
        [HttpGet("{id}")]
        public IActionResult GetPuesto(int id)
        {
            var puesto = nakamaCloudDbContext.Puestos
                .Include(p => p.IdDepartamentoNavigation)
                .FirstOrDefault(p => p.IdPuesto == id);

            if (puesto == null)
                return NotFound(new { message = "Puesto no encontrado" });

            return Ok(new { data = puesto });
        }

        // GET: api/puestos/porDepartamento/5
        [HttpGet("porDepartamento/{idDepartamento}")]
        public IActionResult GetPuestosPorDepartamento(int idDepartamento)
        {
            var puestos = nakamaCloudDbContext.Puestos
                .Where(p => p.IdDepartamento == idDepartamento && p.Activo == true)
                .ToList();

            return Ok(new { data = puestos });
        }

        // POST: api/puestos
        [HttpPost]
        public IActionResult CrearPuesto([FromBody] CrearPuestoDto dto)
        {
            var puesto = new Puesto
            {
                CodigoPuesto = dto.CodigoPuesto,
                NombrePuesto = dto.NombrePuesto,
                DescripcionPuesto = dto.DescripcionPuesto,
                IdDepartamento = dto.IdDepartamento,
                Activo = true
            };

            nakamaCloudDbContext.Puestos.Add(puesto);
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Puesto creado correctamente", data = puesto });
        }

        [HttpPut("{id}")]
        public IActionResult ModificarPuesto(int id, [FromBody] ModificarPuestoDto dto)
        {
            var puesto = nakamaCloudDbContext.Puestos
                .FirstOrDefault(p => p.IdPuesto == id);

            if (puesto == null)
                return NotFound(new { message = "Puesto no encontrado" });

            puesto.NombrePuesto = dto.NombrePuesto;
            puesto.DescripcionPuesto = dto.DescripcionPuesto;
            puesto.IdDepartamento = dto.IdDepartamento;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Puesto modificado correctamente", data = puesto });
        }

        // PATCH: api/puestos/5/desactivar
        [HttpPatch("{id}/desactivar")]
        public IActionResult DesactivarPuesto(int id)
        {
            var puesto = nakamaCloudDbContext.Puestos
                .FirstOrDefault(p => p.IdPuesto == id);

            if (puesto == null)
                return NotFound(new { message = "Puesto no encontrado" });

            puesto.Activo = false;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Puesto desactivado correctamente" });
        }

        // PATCH: api/puestos/5/activar
        [HttpPatch("{id}/activar")]
        public IActionResult ActivarPuesto(int id)
        {
            var puesto = nakamaCloudDbContext.Puestos
                .FirstOrDefault(p => p.IdPuesto == id);

            if (puesto == null)
                return NotFound(new { message = "Puesto no encontrado" });

            puesto.Activo = true;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Puesto activado correctamente" });
        }

        // GET: api/puestos/proximoCodigo
        [HttpGet("proximoCodigo")]
        public IActionResult GetProximoCodigo()
        {
            var ultimoPuesto = nakamaCloudDbContext.Puestos
                .OrderByDescending(e => e.IdPuesto)
                .FirstOrDefault();

            string proximoCodigo;

            if (ultimoPuesto == null)
            {
                proximoCodigo = "PUE-001";
            }
            else
            {
                var ultimoCodigo = ultimoPuesto.CodigoPuesto;
                var numero = int.Parse(ultimoCodigo.Split('-')[1]);
                proximoCodigo = $"PUE-{(numero + 1):D3}";
            }

            return Ok(new { proximoCodigo });
        }

    }
}
