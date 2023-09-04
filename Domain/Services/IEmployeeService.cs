using Domain.Models;

namespace Domain.Services
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<IEmployee>> ImportEmployeesFromCsvAsync(Stream stream);
        public Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync();

        //public IEmployee GetEmployeeById(int employeeId);
        //public Task UpdateEmployee(IEmployee employee);
    }
}
