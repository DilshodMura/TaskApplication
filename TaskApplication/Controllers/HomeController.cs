using AutoMapper;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Import end point.
        /// </summary>
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var importedEmployees = await _service.ImportEmployeesFromCsvAsync(stream);

                        if (importedEmployees.Any())
                        {

                            var employeeViewModels = _mapper.Map<List<EmployeeViewModel>>(importedEmployees);

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

        /// <summary>
        /// Get employee by id endpoint.
        /// </summary>
        [HttpGet]
        [Route("Home/GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            try
            {
                var employee = await _service.GetEmployeeByIdAsync(employeeId);

                if (employee != null)
                {
                    var employeeViewModel = _mapper.Map<EmployeeViewModel>(employee);

                    return PartialView("EditEmployee", employeeViewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = $"Employee with ID {employeeId} not found.";
                    return PartialView("EditEmployee");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("Error");
            }
        }

        /// <summary>
        /// Update employee end point.
        /// </summary>
        [HttpPost]
        [Route("Home/UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee(EmployeeViewModel employeeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("EditEmployee", employeeViewModel);
            }

            try
            {
                var existingEmployee = await _service.GetEmployeeByIdAsync(employeeViewModel.Id);

                if (existingEmployee == null)
                {
                    ViewBag.ErrorMessage = $"Employee with ID {employeeViewModel.Id} not found.";
                    return View("EditEmployee", employeeViewModel);
                }

                _mapper.Map(employeeViewModel, existingEmployee);

                await _service.UpdateEmployeeAsync(existingEmployee);

                TempData["SuccessMessage"] = "Employee updated successfully!";

                return RedirectToAction("ImportCsv");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Update failed: {ex.Message}";
            }

            return View("EditEmployee", employeeViewModel);
        }

    }
}
