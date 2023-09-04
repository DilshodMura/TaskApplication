using Domain.Models;

namespace Domain.Services
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<IEmployee>> ImportEmployeesFromCsvAsync(Stream stream);
        public Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync();

        public Task<IEmployee> GetEmployeeByIdAsync(int employeeId);
        //public Task UpdateEmployee(IEmployee employee);
    }
}
