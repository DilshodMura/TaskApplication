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
            // First, check if the employee with the given ID exists in the database
            var existingEmployee = await _dbContext.Employees.FindAsync(updatedEmployee.Id);

            if (existingEmployee == null)
            {
                throw new Exception($"Employee with ID {updatedEmployee.Id} not found.");
            }

            // Use AutoMapper to map the updated data from the updatedEmployee to the existingEmployee
            _mapper.Map(updatedEmployee, existingEmployee);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<IEmployee>> GetEmployeesByIdsAsync(List<int> employeeIds)
        {
            // Query the database to retrieve employees by their IDs asynchronously
            return await _dbContext.Employees
                .Where(e => employeeIds.Contains(e.Id))
                .Select(e => new EmployeeBusiness
                {
                    Id = e.Id,
                    Payroll_Number = e.Payroll_Number,
                    Forenames = e.Forenames,
                    Surname = e.Surname,
                    EMail_Home = e.EMail_Home,
                    Postcode = e.Postcode,
                    Address = e.Address,
                    Address_2 = e.Address_2,
                    Date_of_Birth = e.Date_of_Birth,
                    Start_Date = e.Start_Date,
                    Mobile = e.Mobile,
                    Telephone = e.Telephone
                })
                .ToListAsync();
        }
    }
}
