using Domain.Models;

namespace Domain.Repositories
{
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Add employee to the database.
        /// </summary>
        public Task AddEmployeesAsync(List<IEmployee> employees);

        /// <summary>
        /// Get employees.
        /// </summary>
        public Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync(int count);

        /// <summary>
        /// Get employee by id.
        /// </summary>
        public Task<IEmployee> GetEmployeeByIdAsync(int employeeId);

        /// <summary>
        /// Update employee.
        /// </summary>
        public Task UpdateEmployeeAsync(IEmployee updatedEmployee);
    }
}