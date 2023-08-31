﻿
using Domain.Models;

namespace Domain.Repositories
{
    public interface IEmployeeRepository
    {
        Task AddEmployeesAsync(List<IEmployee> employees);
        Task<IEnumerable<IEmployee>> GetImportedEmployeesAsync();
    }
}