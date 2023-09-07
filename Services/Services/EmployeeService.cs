using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Service.ServiceModels;
using System.Globalization;

namespace Service.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Import the data from csv file.
        /// </summary>
        public async Task<IEnumerable<IEmployee>> ImportEmployeesFromCsvAsync(Stream stream)
        {
            try
            {   
                var employees = ParseCsvFile(stream);

                var employeeDbList = _mapper.Map<List<IEmployee>>(employees);

                await _employeeRepository.AddEmployeesAsync(employeeDbList);

                return await _employeeRepository.GetImportedEmployeesAsync(employeeDbList.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get employee by id.
        /// </summary>
        public async Task<IEmployee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        }

        public async Task UpdateEmployeeAsync(IEmployee updatedEmployee)
        {
            await _employeeRepository.UpdateEmployeeAsync(updatedEmployee);
        }


        /// <summary>
        /// parsing csv file
        /// </summary>
        private IEnumerable<IEmployee> ParseCsvFile(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.Context.RegisterClassMap<EmployeeServiceModelMap>();

                return csv.GetRecords<EmployeeServiceModel>().ToList();
            }
        }
    }
}
