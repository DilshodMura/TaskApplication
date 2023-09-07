using Domain.Models;

namespace Domain.Services
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Import employee from csv file
        /// </summary>
        public Task<IEnumerable<IEmployee>> ImportEmployeesFromCsvAsync(Stream stream);

        /// <summary>
        /// Get employee by id
        /// </summary>
        public Task<IEmployee> GetEmployeeByIdAsync(int employeeId);

        /// <summary>
        /// Update Employee
        /// </summary>
        public Task UpdateEmployeeAsync(IEmployee employee);
    }
}
