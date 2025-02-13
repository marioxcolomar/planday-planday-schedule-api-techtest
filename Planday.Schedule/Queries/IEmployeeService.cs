using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries
{
  public interface IEmployeeService
  {
    Task<IReadOnlyCollection<Employee>> GetAllEmployees();
    Task<Employee> GetEmployeeById(long Id);
  }
}

