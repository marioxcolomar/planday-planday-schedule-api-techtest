using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Planday.Schedule.Infrastructure.Queries;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Api.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class EmployeeController : ControllerBase
  {
    private readonly IEmployeeService _employeeService;
    public EmployeeController(IEmployeeService employeeService)
    {
      _employeeService = employeeService;
    }

    [HttpGet]
    public Task<IReadOnlyCollection<Employee>> GetAllEmployees()
    {
      return _employeeService.GetAllEmployees();
    }

    [HttpGet("{id}")]
    public ActionResult<Employee> GetEmployeeById(long id)
    {
      var employee = _employeeService.GetEmployeeById(id);
      if (employee == null)
      {
        return NotFound("Record not found.");
      }
      return Ok(employee.Result);
    }
  }

}
