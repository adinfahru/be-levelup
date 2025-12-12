using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee> GetEmployeeByIdAsync(Guid employeeId);
    Task<IEnumerable<Employee>> GetAllEmployees();
    new Task<bool> UpdateAsync(Employee employee);
}
