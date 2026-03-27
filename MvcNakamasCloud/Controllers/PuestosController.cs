using Microsoft.AspNetCore.Mvc;
using MvcNakamasCloud.ViewModels;
using System.Text.Json;

namespace MvcNakamasCloud.Controllers
{
    public class PuestosController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerOptions jsonOptions;

        public PuestosController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // GET: /Puestos
        public async Task<IActionResult> Index()
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/puestos");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleResponse<List<PuestoViewModel>>>(content, jsonOptions);

            return View(result.Data);
        }

        // GET: /Puestos/Crear
        public async Task<IActionResult> Crear()
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/puestos/proximoCodigo");
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var codigo = jsonDoc.RootElement.GetProperty("proximoCodigo").GetString();

            return View(new PuestoFormViewModel
            {
                CodigoPuesto = codigo,
                Departamentos = await GetDepartamentos()
            });
        }

        // POST: /Puestos/Crear
        [HttpPost]
        public async Task<IActionResult> Crear(PuestoFormViewModel puesto)
        {
            if (!ModelState.IsValid)
            {
                puesto.Departamentos = await GetDepartamentos();
                return View(puesto);
            }

            var client = httpClientFactory.CreateClient("NakamaApi");
            var json = JsonSerializer.Serialize(new
            {
                puesto.CodigoPuesto,
                puesto.NombrePuesto,
                puesto.DescripcionPuesto,
                puesto.IdDepartamento
            });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/puestos", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = "Ocurrió un error al crear el puesto";
            puesto.Departamentos = await GetDepartamentos();
            return View(puesto);
        }

        // GET: /Puestos/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/puestos");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleResponse<List<PuestoViewModel>>>(content, jsonOptions);

            var puesto = result.Data.FirstOrDefault(p => p.IdPuesto == id);

            var formViewModel = new PuestoFormViewModel
            {
                IdPuesto = puesto.IdPuesto,
                CodigoPuesto = puesto.CodigoPuesto,
                NombrePuesto = puesto.NombrePuesto,
                DescripcionPuesto = puesto.DescripcionPuesto,
                IdDepartamento = puesto.IdDepartamento,
                Departamentos = await GetDepartamentos()
            };

            return View(formViewModel);
        }

        // POST: /Puestos/Editar
        [HttpPost]
        public async Task<IActionResult> Editar(PuestoFormViewModel puesto)
        {
            if (!ModelState.IsValid)
            {
                puesto.Departamentos = await GetDepartamentos();
                return View(puesto);
            }

            var client = httpClientFactory.CreateClient("NakamaApi");
            var json = JsonSerializer.Serialize(new
            {
                puesto.NombrePuesto,
                puesto.DescripcionPuesto,
                puesto.IdDepartamento
            });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/puestos/{puesto.IdPuesto}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = "Ocurrió un error al modificar el puesto";
            puesto.Departamentos = await GetDepartamentos();
            return View(puesto);
        }

        // GET: /Puestos/Activar/5
        public async Task<IActionResult> Activar(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            await client.PatchAsync($"api/puestos/{id}/activar", null);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Puestos/Desactivar/5
        public async Task<IActionResult> Desactivar(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            await client.PatchAsync($"api/puestos/{id}/desactivar", null);
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para obtener departamentos
        private async Task<List<DepartamentoViewModel>> GetDepartamentos()
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/departamentos");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleResponse<List<DepartamentoViewModel>>>(content, jsonOptions);
            return result.Data.Where(d => d.Activo).ToList();
        }
    }
}
