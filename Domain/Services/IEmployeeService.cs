using Domain.Models;
using Domain.Repositories;

namespace Domain.Services
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<IEmployee>> ParseCsvFile(Stream stream);
        public Task ImportEmployeesFromCsvAsync(Stream stream);
        public Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync();

        public IEmployee GetEmployeeById(int employeeId);
        public Task UpdateEmployee(IEmployee employee);
    }
}
