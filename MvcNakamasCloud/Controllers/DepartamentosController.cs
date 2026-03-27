using Microsoft.AspNetCore.Mvc;
using MvcNakamasCloud.ViewModels;
using System.Text.Json;

namespace MvcNakamasCloud.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerOptions jsonOptions;

        public DepartamentosController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // GET: /Departamentos
        public async Task<IActionResult> Index()
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/departamentos");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleResponse<List<DepartamentoViewModel>>>(content, jsonOptions);

            return View(result.Data);
        }

        // GET: /Departamentos/Crear
        public async Task<IActionResult> Crear()
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/departamentos/proximoCodigo");
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var codigo = jsonDoc.RootElement.GetProperty("proximoCodigo").GetString();

            return View(new DepartamentoFormViewModel
            {
                CodigoDepartamento = codigo
            });
        }

        // POST: /Departamentos/Crear
        [HttpPost]
        public async Task<IActionResult> Crear(DepartamentoFormViewModel departamento)
        {
            if (!ModelState.IsValid)
                return View(departamento);

            var client = httpClientFactory.CreateClient("NakamaApi");
            var json = JsonSerializer.Serialize(departamento);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/departamentos", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = "Ocurrió un error al crear el departamento";
            return View(departamento);
        }

        // GET: /Departamentos/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            var response = await client.GetAsync("api/departamentos");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleResponse<List<DepartamentoViewModel>>>(content, jsonOptions);

            var departamento = result.Data.FirstOrDefault(d => d.IdDepartamento == id);

            var formViewModel = new DepartamentoFormViewModel
            {
                IdDepartamento = departamento.IdDepartamento,
                CodigoDepartamento = departamento.CodigoDepartamento,
                NombreDepartamento = departamento.NombreDepartamento
            };

            return View(formViewModel);
        }

        // POST: /Departamentos/Editar
        [HttpPost]
        public async Task<IActionResult> Editar(DepartamentoFormViewModel departamento)
        {
            if (!ModelState.IsValid)
                return View(departamento);

            var client = httpClientFactory.CreateClient("NakamaApi");
            var json = JsonSerializer.Serialize(departamento);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"api/departamentos/{departamento.IdDepartamento}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = "Ocurrió un error al modificar el departamento";
            return View(departamento);
        }

        // GET: /Departamentos/Activar/5
        public async Task<IActionResult> Activar(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            await client.PatchAsync($"api/departamentos/{id}/activar", null);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Departamentos/Desactivar/5
        public async Task<IActionResult> Desactivar(int id)
        {
            var client = httpClientFactory.CreateClient("NakamaApi");
            await client.PatchAsync($"api/departamentos/{id}/desactivar", null);
            return RedirectToAction(nameof(Index));
        }
    }
}
