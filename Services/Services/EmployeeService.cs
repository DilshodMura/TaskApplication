using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Models;
using Domain.Repositories;
using System.Globalization;

namespace Service.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<IEnumerable<IEmployee>> ParseCsvFile(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                // Parse CSV records
                var records = csv.GetRecords<EmployeeService>().ToList();

                // Map CSV records to IEmployee objects using AutoMapper
                var employees = _mapper.Map<List<IEmployee>>(records);

                return employees;
            }
        }

        public async Task ImportEmployeesFromCsvAsync(Stream stream)
        {
            var employees = await ParseCsvFile(stream);

            await _employeeRepository.AddEmployeesAsync(employees.ToList());
        }

        public async Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync()
        {
            return await _employeeRepository.GetImportedEmployeesAsync();
        }
    }
}
