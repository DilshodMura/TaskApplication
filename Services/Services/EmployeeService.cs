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

        private async Task<IEnumerable<IEmployee>> ParseCsvFile(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.Context.RegisterClassMap<EmployeeServiceModelMap>(); // Register the custom class map

                // Parse CSV records
                var records = csv.GetRecords<EmployeeServiceModel>().ToList();

                // Map CSV records to IEmployee objects using AutoMapper
                var employees = _mapper.Map<List<IEmployee>>(records);

                return employees;
            }
        }

        public async Task<IEnumerable<IEmployee>> ImportEmployeesFromCsvAsync(Stream stream)
        {
            try
            {
                var employees = await ParseCsvFile(stream);

                var employeeDbList = _mapper.Map<List<IEmployee>>(employees);

                int count = employeeDbList.Count();

                await _employeeRepository.AddEmployeesAsync(employeeDbList);

                // Call the modified GetImportedEmployeesAsync method with the count.
                return await GetImportedEmployeesAsync(count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("test");
                return null;
            }
        }

        public async Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync(int count)
        {
            return await _employeeRepository.GetImportedEmployeesAsync(count);
        }

        public async Task<IEmployee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        }

        public async Task UpdateEmployeeAsync(IEmployee updatedEmployee)
        {
            await _employeeRepository.UpdateEmployeeAsync(updatedEmployee);
        }

        public async Task<IEnumerable<IEmployee>> GetEmployeesByIdsAsync(List<int> employeeIds)
        {
            // Delegate the data retrieval to the repository layer asynchronously
            return await _employeeRepository.GetEmployeesByIdsAsync(employeeIds);
        }
    }
}
