using Microsoft.AspNetCore.Mvc;
using MvcNakamasCloud.ViewModels;
using System.Text.Json;

namespace MvcNakamasCloud.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerOptions jsonOptions;

        public EmpleadosController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // GET: /Empleados
        public async Task<IActionResult> Index(
            string? nombre = null,
            string? apellido = null,
            string? fechaContratacion = null,
            int page = 1,
            int pageSize = 10)
        {
            nombre = string.IsNullOrEmpty(nombre) ? null : nombre;
            apellido = string.IsNullOrEmpty(apellido) ? null : apellido;
            fechaContratacion = string.IsNullOrEmpty(fechaContratacion) ? null : fechaContratacion;

            var client = httpClientFactory.CreateClient("NakamaApi");
            var url = $"api/empleados?page={page}&pageSize={pageSize}";

            if (!string.IsNullOrEmpty(nombre)) url += $"&nombre={nombre}";
            if (!string.IsNullOrEmpty(apellido)) url += $"&apellido={apellido}";
            if (!string.IsNullOrEmpty(fechaContratacion)) url += $"&fechaContratacion={fechaContratacion}";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaginationResponse<EmpleadoViewModel>>(content, jsonOptions);

            var viewModel = new EmpleadoListViewModel
            {
                Empleados = result.Data,
                Pagination = new PaginationInfo
                {
                    Page = result.Page,
                    PageSize = result.PageSize,
                    Total = result.Total,
                    Pages = result.Pages
                },
                FiltroNombre = nombre,
                FiltroApellido = apellido,
                FiltroFechaContratacion = fechaContratacion
            };

            return View(viewModel);
        }

        // GET: /Empleados/Crear
        public async Task<IActionResult> Crear()
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/empleados/proximoCodigo");
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var codigo = jsonDoc.RootElement.GetProperty("proximoCodigo").GetString();

            return View(new EmpleadoFormViewModel
            {
                CodigoEmpleado = codigo,
                FechaContratacion = DateTime.Now.ToString("yyyy-MM-dd"),
                Departamentos = await GetDepartamentos(),
                Puestos = await GetPuestos()
            });
        }

        // POST: /Empleados/Crear
        [HttpPost]
        public async Task<IActionResult> Crear(EmpleadoFormViewModel empleado)
        {
            if (!ModelState.IsValid)
            {
                empleado.Departamentos = await GetDepartamentos();
                empleado.Puestos = await GetPuestos();
                return View(empleado);
            }

            var client = httpClientFactory.CreateClient("NakamaApi");

            // 1 — Crear empleado
            var empleadoJson = JsonSerializer.Serialize(new
            {
                empleado.CodigoEmpleado,
                empleado.PrimerNombre,
                empleado.SegundoNombre,
                empleado.PrimerApellido,
                empleado.SegundoApellido,
                empleado.FechaNacimiento,
                empleado.FechaContratacion,
                empleado.Direccion,
                empleado.Telefono,
                empleado.Email
            });
            var empleadoContent = new StringContent(empleadoJson, System.Text.Encoding.UTF8, "application/json");
            var empleadoResponse = await client.PostAsync("api/empleados", empleadoContent);

            if (!empleadoResponse.IsSuccessStatusCode)
            {
                var errorBody = await empleadoResponse.Content.ReadAsStringAsync();
                ViewBag.Error = $"Error al crear empleado: {errorBody}";
                empleado.Departamentos = await GetDepartamentos();
                empleado.Puestos = await GetPuestos();
                return View(empleado);
            }
            var responseBody = await empleadoResponse.Content.ReadAsStringAsync();

            // 2 — Obtener ID del empleado creado

            var jsonDoc = JsonDocument.Parse(responseBody);
            var nuevoId = jsonDoc.RootElement.GetProperty("data").GetProperty("idEmpleado").GetInt32();

            // 3 — Crear asignacion de puesto
            var asignacionJson = JsonSerializer.Serialize(new
            {
                idEmpleado = nuevoId,
                idPuesto = empleado.IdPuesto,
                salario = empleado.Salario ?? 0,
                fechaInicio = empleado.FechaContratacion
            });
            var asignacionContent = new StringContent(asignacionJson, System.Text.Encoding.UTF8, "application/json");
            var asignacionResponse = await client.PostAsync("api/asignacionPuestos", asignacionContent);
            var asignacionBody = await asignacionResponse.Content.ReadAsStringAsync();

            TempData["Debug"] = $"ID:{nuevoId} | Puesto:{empleado.IdPuesto} | Salario:{empleado.Salario} | Status:{asignacionResponse.StatusCode} | Respuesta:{asignacionBody}";

            return RedirectToAction(nameof(Editar), new { id = nuevoId });
        }

        // GET: /Empleados/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync($"api/empleados/{id}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleResponse<EmpleadoViewModel>>(content, jsonOptions);

            var empleado = result.Data;
            var asignacionActiva = empleado.AsignacionPuestos?.FirstOrDefault(a => a.Estado == "Activo");

            var formViewModel = new EmpleadoFormViewModel
            {
                IdEmpleado = id,
                CodigoEmpleado = empleado.CodigoEmpleado,
                PrimerNombre = empleado.PrimerNombre,
                SegundoNombre = empleado.SegundoNombre,
                PrimerApellido = empleado.PrimerApellido,
                SegundoApellido = empleado.SegundoApellido,
                FechaNacimiento = empleado.FechaNacimiento.ToString("yyyy-MM-dd"),
                FechaContratacion = empleado.FechaContratacion.ToString("yyyy-MM-dd"),
                Direccion = empleado.Direccion,
                Telefono = empleado.Telefono,
                Email = empleado.Email,
                FotoRuta = empleado.FotoRuta,
                IdPuesto = asignacionActiva?.IdPuesto ?? 0,
                IdDepartamento = asignacionActiva?.IdPuestoNavigation?.IdDepartamento ?? 0,
                Salario = asignacionActiva?.Salario ?? 0,
                Departamentos = await GetDepartamentos(),
                Puestos = await GetPuestos()
            };

            ViewBag.FotoRuta = empleado.FotoRuta;
            return View(formViewModel);
        }

        // POST: /Empleados/Editar
        [HttpPost]
        public async Task<IActionResult> Editar(EmpleadoFormViewModel empleado)
        {
            if (!ModelState.IsValid)
            {
                empleado.Departamentos = await GetDepartamentos();
                empleado.Puestos = await GetPuestos();
                return View(empleado);
            }

            var client = httpClientFactory.CreateClient("NakamaApi");

            // 1 — Actualizar empleado
            var empleadoJson = JsonSerializer.Serialize(new
            {
                empleado.CodigoEmpleado,
                empleado.PrimerNombre,
                empleado.SegundoNombre,
                empleado.PrimerApellido,
                empleado.SegundoApellido,
                empleado.FechaNacimiento,
                empleado.FechaContratacion,
                empleado.Direccion,
                empleado.Telefono,
                empleado.Email
            });
            var empleadoContent = new StringContent(empleadoJson, System.Text.Encoding.UTF8, "application/json");
            var empleadoResponse = await client.PutAsync($"api/empleados/{empleado.IdEmpleado}", empleadoContent);

            if (!empleadoResponse.IsSuccessStatusCode)
            {
                var errorBody = await empleadoResponse.Content.ReadAsStringAsync();
                ViewBag.Error = $"Error al modificar empleado: {errorBody}";
                empleado.Departamentos = await GetDepartamentos();
                empleado.Puestos = await GetPuestos();
                return View(empleado);
            }

            // 2 — Actualizar asignacion de puesto
            var asignacionJson = JsonSerializer.Serialize(new
            {
                idPuesto = empleado.IdPuesto,
                salario = empleado.Salario,
                fechaInicio = empleado.FechaContratacion
            });
            var asignacionContent = new StringContent(asignacionJson, System.Text.Encoding.UTF8, "application/json");
            await client.PutAsync($"api/asignacionPuestos/porEmpleado/{empleado.IdEmpleado}", asignacionContent);


            return RedirectToAction(nameof(Index));
        }

        // PATCH: /Empleados/Activar/5
        public async Task<IActionResult> Activar(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            await client.PatchAsync($"api/empleados/{id}/activar", null);
            return RedirectToAction(nameof(Index));
        }

        // PATCH: /Empleados/Desactivar/5
        public async Task<IActionResult> Desactivar(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            await client.PatchAsync($"api/empleados/{id}/desactivar", null);
            return RedirectToAction(nameof(Index));
        }

        // PATCH: /Empleados/Despedir/5
        public async Task<IActionResult> Despedir(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            await client.PatchAsync($"api/empleados/{id}/despedir", null);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Empleados/CargarFoto
        [HttpPost]
        public async Task<IActionResult> CargarFoto(int idEmpleado, IFormFile foto)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var formContent = new MultipartFormDataContent();
            var fileStream = foto.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(foto.ContentType);
            formContent.Add(fileContent, "foto", foto.FileName);

            var response = await client.PostAsync($"api/empleados/{idEmpleado}/foto", formContent);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Editar), new { id = idEmpleado });

            ViewBag.Error = "Ocurrió un error al cargar la fotografía";
            return RedirectToAction(nameof(Editar), new { id = idEmpleado });
        }

        private async Task<List<DepartamentoViewModel>> GetDepartamentos()
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/departamentos");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleResponse<List<DepartamentoViewModel>>>(content, jsonOptions);
            return result.Data.Where(d => d.Activo).ToList();
        }

        private async Task<List<PuestoViewModel>> GetPuestos()
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/puestos");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleResponse<List<PuestoViewModel>>>(content, jsonOptions);
            return result.Data.Where(p => p.Activo).ToList();
        }

        // GET: /Empleados/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync($"api/empleados/{id}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleResponse<EmpleadoViewModel>>(content, jsonOptions);

            return View(result.Data);
        }
    }
}