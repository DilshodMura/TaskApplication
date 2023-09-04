using AutoMapper;
using Database.DbContexts;
using Database.Entities;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync()
        {
            var employeeEntities = await _dbContext.Employees.ToListAsync();
            return _mapper.Map<List<IEmployee>>(employeeEntities);
        }

        public async Task<IEmployee> GetEmployeeByIdAsync(int employeeId)
        {
            var employeeEntity = await _dbContext.Employees.FindAsync(employeeId);

            if (employeeEntity == null)
            {
                throw new Exception($"Employee with ID {employeeId} not found.");
            }

            return _mapper.Map<IEmployee>(employeeEntity);
        }
        public async Task UpdateEmployeeAsync(IEmployee employee)
        {
            var employeeEntity = await _dbContext.Employees.FindAsync(employee.Id);

            if (employeeEntity == null)
            {
                throw new Exception($"Employee with ID {employee.Id} not found for update.");
            }

            // Map properties from IEmployee to EmployeeDb
            _mapper.Map(employee, employeeEntity);

            await _dbContext.SaveChangesAsync();
        }

    }
}
