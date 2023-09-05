
using Domain.Models;

namespace Domain.Repositories
{
    public interface IEmployeeRepository
    {
        public Task AddEmployeesAsync(List<IEmployee> employees);
        public Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync(int count);
        public Task<IEmployee> GetEmployeeByIdAsync(int employeeId);
        public Task UpdateEmployeeAsync(IEmployee updatedEmployee);
    }
}