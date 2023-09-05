using Domain.Models;

namespace Domain.Services
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<IEmployee>> ImportEmployeesFromCsvAsync(Stream stream);
        public Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync(int count);

        public Task<IEmployee> GetEmployeeByIdAsync(int employeeId);
        public Task UpdateEmployeeAsync(IEmployee employee);
    }
}
