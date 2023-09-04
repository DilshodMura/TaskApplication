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

                await _employeeRepository.AddEmployeesAsync(employeeDbList);

                return employeeDbList.Select(employee => _mapper.Map<EmployeeServiceModel>(employee));
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<EmployeeServiceModel>();
            }
        }


        public async Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync()
        {
            return await _employeeRepository.GetImportedEmployeesAsync();
        }
    }
}
