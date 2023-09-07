using AutoMapper;
using Database.DbContexts;
using Database.Entities;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Repository.BusinessModels;

namespace Repository.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public EmployeeRepository(AppDbContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Add employee to database.
        /// </summary>
        public async Task AddEmployeesAsync(List<IEmployee> employees)
        {
            var employeeEntities = _mapper.Map<List<EmployeeDb>>(employees);
            _dbContext.Employees.AddRange(employeeEntities);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get employee from database.
        /// </summary>
        public async Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync(int count)
        {
            var employeeEntities = await _dbContext.Employees
                .OrderBy(e => e.Id) 
                .Skip(Math.Max(0, await _dbContext.Employees.CountAsync() - count))
                .ToListAsync();

            return _mapper.Map<List<EmployeeBusiness>>(employeeEntities);
        }

        /// <summary>
        /// Get employee by id from database.
        /// </summary>
        public async Task<IEmployee> GetEmployeeByIdAsync(int employeeId)
        {
            var employeeEntity = await _dbContext.Employees.FindAsync(employeeId);

            return employeeEntity == null
                ? throw new Exception($"Employee with ID {employeeId} not found.")
                : _mapper.Map<EmployeeBusiness>(employeeEntity);
        }

        /// <summary>
        /// Update employee from database.
        /// </summary>
        public async Task UpdateEmployeeAsync(IEmployee updatedEmployee)
        {
            var existingEmployee = await _dbContext.Employees.FindAsync(updatedEmployee.Id);

            if (existingEmployee == null)
            {
                throw new Exception($"Employee with ID {updatedEmployee.Id} not found.");
            }

            _mapper.Map(updatedEmployee, existingEmployee);

            await _dbContext.SaveChangesAsync();
        }
    }
}
