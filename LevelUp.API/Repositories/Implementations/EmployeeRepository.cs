using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(LevelUpDbContext context) : base(context)
    {
    }

    public async Task<Employee> GetEmployeeByIdAsync(Guid employeeId)
    {
         return await _context.Employees
            .Include(e => e.Account)
            .FirstAsync(e => e.Id == employeeId);
    }

    public async Task<IEnumerable<Employee>> GetAllEmployees()
    {
        return await _context.Employees
            .Include(e => e.Account)
            .ToListAsync();
    }

    public new async Task<bool> UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        return await _context.SaveChangesAsync() > 0;
    }


}
