using AutoMapper;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace TaskApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeService _service;
        private readonly IMapper _mapper;

        public HomeController(IEmployeeService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        // Perform the CSV import and get the list of imported employees
                        var importedEmployees = await _service.ImportEmployeesFromCsvAsync(stream);

                        // Display a success message and pass the imported employees to the view
                        TempData["SuccessMessage"] = "Import successful!";
                        return View("Import", importedEmployees);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Import failed: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Message = "No file selected.";
            }

            return View("Import");
        }

        [HttpGet]
        public async Task<IActionResult> GetImportedEmployees()
        {
            try
            {
                // Retrieve imported employees from the service
                var employees = await _service.GetImportedEmployeesAsync();
                return View(employees);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
                return View("Import");
            }
        }
    }
}
