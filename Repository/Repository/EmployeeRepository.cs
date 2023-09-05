using AutoMapper;
using Database.DbContexts;
using Database.Entities;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Repository.BusinessModels;
using Service.ServiceModels;

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

        public async Task AddEmployeesAsync(List<IEmployee> employees)
        {
            var employeeEntities = _mapper.Map<List<EmployeeDb>>(employees);
            _dbContext.Employees.AddRange(employeeEntities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync(int count)
        {
            var employeeEntities = await _dbContext.Employees
                .OrderBy(e => e.Id) // Assuming there's an "Id" property that represents the order
                .Skip(Math.Max(0, await _dbContext.Employees.CountAsync() - count)) // Skip all but the last two
                .ToListAsync();

            return _mapper.Map<List<EmployeeBusiness>>(employeeEntities);
        }

        public async Task<IEmployee> GetEmployeeByIdAsync(int employeeId)
        {
            var employeeEntity = await _dbContext.Employees.FindAsync(employeeId);

            return employeeEntity == null
                ? throw new Exception($"Employee with ID {employeeId} not found.")
                : _mapper.Map<EmployeeBusiness>(employeeEntity);
        }
        public async Task UpdateEmployeeAsync(IEmployee updatedEmployee)
        {
            // Instead of creating a new instance, work with the existing tracked entity
            var entry = _dbContext.Entry(updatedEmployee);

            if (entry.State == EntityState.Detached)
            {
                // This condition should not be necessary, but you can keep it for extra safety
                _dbContext.Attach(updatedEmployee);
            }

            // The entity is already tracked, so there's no need to update it explicitly
            await _dbContext.SaveChangesAsync();
        }
    }
}
