using APIServidorEmpleados.Data;
using APIServidorEmpleados.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIServidorEmpleados.DTOs;

namespace APIServidorEmpleados.Controllers
{
    [Route("api/empleados")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private readonly NakamaCloudDbContext nakamaCloudDbContext;

        public EmpleadosController(NakamaCloudDbContext nakamaCloudDbContext)
        {
            this.nakamaCloudDbContext = nakamaCloudDbContext;
        }

        // GET: api/empleados
        [HttpGet]
        public IActionResult GetEmpleados(
            string? nombre = null,
            string? apellido = null,
            DateOnly? fechaContratacion = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = nakamaCloudDbContext.Empleados
                .Include(e => e.AsignacionPuestos.Where(a => a.Estado == "Activo"))
                    .ThenInclude(a => a.IdPuestoNavigation)
                        .ThenInclude(p => p.IdDepartamentoNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(e => e.PrimerNombre.Contains(nombre) ||
                                         e.SegundoNombre.Contains(nombre));

            if (!string.IsNullOrEmpty(apellido))
                query = query.Where(e => e.PrimerApellido.Contains(apellido) ||
                                         e.SegundoApellido.Contains(apellido));

            if (fechaContratacion.HasValue)
                query = query.Where(e => e.FechaContratacion == fechaContratacion.Value);

            var total = query.Count();

            var response = new PaginationDto<Empleado>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Pages = (int)Math.Ceiling((double)total / pageSize),
                Data = query
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
            };

            return Ok(response);
        }

        // GET: api/empleados/5
        [HttpGet("{id}")]
        public IActionResult GetEmpleado(int id)
        {
            var empleado = nakamaCloudDbContext.Empleados
                .Include(e => e.AsignacionPuestos.Where(a => a.Estado == "Activo"))
                    .ThenInclude(a => a.IdPuestoNavigation)
                        .ThenInclude(p => p.IdDepartamentoNavigation)
                .FirstOrDefault(e => e.IdEmpleado == id);

            if (empleado == null)
                return NotFound(new { message = "Empleado no encontrado" });

            return Ok(new { data = empleado });
        }

        // POST: api/empleados
        [HttpPost]
        public IActionResult CrearEmpleado([FromBody] CrearEmpleadoDto dto)
        {
            var empleado = new Empleado
            {
                CodigoEmpleado = dto.CodigoEmpleado,
                PrimerNombre = dto.PrimerNombre,
                SegundoNombre = dto.SegundoNombre,
                PrimerApellido = dto.PrimerApellido,
                SegundoApellido = dto.SegundoApellido,
                FechaNacimiento = dto.FechaNacimiento,
                FechaContratacion = dto.FechaContratacion,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Estado = "Activo",
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            nakamaCloudDbContext.Empleados.Add(empleado);
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Empleado creado correctamente", data = empleado });
        }

        // PUT: api/empleados/5
        [HttpPut("{id}")]
        public IActionResult ModificarEmpleado(int id, [FromBody] ModificarEmpleadoDto dto)
        {
            var empleado = nakamaCloudDbContext.Empleados
                .FirstOrDefault(e => e.IdEmpleado == id);

            if (empleado == null)
                return NotFound(new { message = "Empleado no encontrado" });

            empleado.PrimerNombre = dto.PrimerNombre;
            empleado.SegundoNombre = dto.SegundoNombre;
            empleado.PrimerApellido = dto.PrimerApellido;
            empleado.SegundoApellido = dto.SegundoApellido;
            empleado.FechaNacimiento = dto.FechaNacimiento;
            empleado.FechaContratacion = dto.FechaContratacion;
            empleado.Direccion = dto.Direccion;
            empleado.Telefono = dto.Telefono;
            empleado.Email = dto.Email;
            empleado.FechaModificacion = DateTime.Now;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Empleado modificado correctamente", data = empleado });
        }

        // PATCH: api/empleados/5/desactivar
        [HttpPatch("{id}/desactivar")]
        public IActionResult DesactivarEmpleado(int id)
        {
            var empleado = nakamaCloudDbContext.Empleados
                .FirstOrDefault(e => e.IdEmpleado == id);

            if (empleado == null)
                return NotFound(new { message = "Empleado no encontrado" });

            empleado.Estado = "Inactivo";
            empleado.FechaModificacion = DateTime.Now;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Empleado desactivado correctamente" });
        }

        [HttpPatch("{id}/activar")]
        public IActionResult ActivarEmpleado(int id)
        {
            var empleado = nakamaCloudDbContext.Empleados
                .FirstOrDefault(e => e.IdEmpleado == id);

            if (empleado == null)
                return NotFound(new { message = "Empleado no encontrado" });

            empleado.Estado = "Activo";
            empleado.FechaModificacion = DateTime.Now;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Empleado activado correctamente" });
        }

        // PATCH: api/empleados/5/despedir
        [HttpPatch("{id}/despedir")]
        public IActionResult DespedirEmpleado(int id)
        {
            var empleado = nakamaCloudDbContext.Empleados
                .Include(e => e.AsignacionPuestos.Where(a => a.Estado == "Activo"))
                .FirstOrDefault(e => e.IdEmpleado == id);

            if (empleado == null)
                return NotFound(new { message = "Empleado no encontrado" });

            // Cambia estado del empleado
            empleado.Estado = "Despedido";
            empleado.FechaModificacion = DateTime.Now;

            // Cancela la asignacion de puesto activa
            foreach (var asignacion in empleado.AsignacionPuestos)
            {
                asignacion.Estado = "Cancelado";
                asignacion.FechaFin = DateOnly.FromDateTime(DateTime.Now);
            }

            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Empleado despedido correctamente" });
        }

        // POST: api/empleados/5/foto
        [HttpPost("{id}/foto")]
        public IActionResult CargarFoto(int id, IFormFile foto)
        {
            var empleado = nakamaCloudDbContext.Empleados
                .FirstOrDefault(e => e.IdEmpleado == id);

            if (empleado == null)
                return NotFound(new { message = "Empleado no encontrado" });

            // Validar que no venga vacio
            if (foto == null || foto.Length == 0)
                return BadRequest(new { message = "El archivo está vacío" });

            // Validar tamaño 10MB
            const long maxFileSize = 10 * 1024 * 1024;
            if (foto.Length > maxFileSize)
                return BadRequest(new { message = "El archivo no puede superar los 10MB" });

            // Validar extension
            var extension = Path.GetExtension(foto.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Solo se permiten archivos jpg, jpeg y png" });

            // Generar nombre
            var fileName = Guid.NewGuid().ToString() + extension;

            // Construir ruta donde se guarda
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Crear carpeta si no existe
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Guardar archivo en disco
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                foto.CopyTo(stream);
            }

            // Guardar ruta relativa
            empleado.FotoRuta = "uploads/" + fileName;
            empleado.FechaModificacion = DateTime.Now;
            nakamaCloudDbContext.SaveChanges();

            return Ok(new { message = "Fotografía cargada correctamente", fotoRuta = empleado.FotoRuta });
        }

        // GET: api/empleados/proximoCodigo
        [HttpGet("proximoCodigo")]
        public IActionResult GetProximoCodigo()
        {
            var ultimoEmpleado = nakamaCloudDbContext.Empleados
                .OrderByDescending(e => e.IdEmpleado)
                .FirstOrDefault();

            string proximoCodigo;

            if (ultimoEmpleado == null)
            {
                proximoCodigo = "EMP-001";
            }
            else
            {
                var ultimoCodigo = ultimoEmpleado.CodigoEmpleado;
                var numero = int.Parse(ultimoCodigo.Split('-')[1]);
                proximoCodigo = $"EMP-{(numero + 1):D3}";
            }

            return Ok(new { proximoCodigo });
        }

    }
}
