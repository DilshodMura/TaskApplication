using AutoMapper;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Service.ServiceModels;
using TaskApplication.Models;

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

                        // Check if any employees were imported
                        if (importedEmployees.Any())
                        {
                            // Configure AutoMapper for EmployeeServiceModel to EmployeeViewModel mapping
                            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<EmployeeServiceModel, EmployeeViewModel>();
                            });

                            // Create an IMapper instance using the configured mappings
                            IMapper mapper = config.CreateMapper();

                            // Map the imported employees to EmployeeViewModel using the specific mapper
                            //var employeeViewModels = mapper.Map<List<EmployeeViewModel>>(importedEmployees);
                            var employeeViewModels = GetImportedEmployees();

                            // Display a success message and pass the EmployeeViewModels to the view
                            TempData["SuccessMessage"] = $"Import successful! {employeeViewModels} rows processed.";
                            return View("Import", employeeViewModels);
                        }
                        else
                        {
                            ViewBag.Message = "No employees were imported from the CSV file.";
                        }
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
                // Retrieve employees from the database
                var employees = await _service.GetImportedEmployeesAsync();

                // Map the employees to EmployeeViewModel using AutoMapper
                var employeeViewModels = _mapper.Map<List<EmployeeViewModel>>(employees);

                // Pass the mapped data to the view
                return View(employeeViewModels);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
                return View("Import");
            }
        }

        [HttpGet]
        [Route("Home/GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            try
            {
                // Retrieve the employee by ID from the service
                var employee = await _service.GetEmployeeByIdAsync(employeeId);

                if (employee != null)
                {
                    // Map the employee to an EmployeeViewModel
                    var employeeViewModel = _mapper.Map<EmployeeViewModel>(employee);

                    return PartialView("EditEmployee", employeeViewModel);
                }
                else
                {
                    // Handle the case when an employee is not found
                    ViewBag.ErrorMessage = $"Employee with ID {employeeId} not found.";
                    return PartialView("EditEmployee");
                }
            }
            catch (Exception ex)
            {
                // Handle the custom NotFoundException and display the error message
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("EditEmployee");
            }
        }
    }
}
