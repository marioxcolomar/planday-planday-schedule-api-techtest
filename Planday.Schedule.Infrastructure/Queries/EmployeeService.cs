using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Exceptions;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
  public class EmployeeService : IEmployeeService
  {
    private readonly IConnectionStringProvider _connectionStringProvider;

    public EmployeeService(IConnectionStringProvider connectionStringProvider)
    {
      _connectionStringProvider = connectionStringProvider;
    }

    public async Task<IReadOnlyCollection<Employee>> GetAllEmployees()
    {
      await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

      var employeeDtos = await sqlConnection.QueryAsync<EmployeeDto>(queryAll);

      var employee = employeeDtos.Select(x =>
          new Employee(x.Id, x.Name));

      return employee.ToList();
    }

    private const string queryAll = @"SELECT Id, Name FROM Employee;";

    private record EmployeeDto(long Id, string Name);

    public async Task<Employee> GetEmployeeById(long id)
    {
      await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
      sqlConnection.Open();

      Employee? employee = null;
      using (var command = new SqliteCommand(queryById, sqlConnection))
      {
        command.Parameters.AddWithValue("@id", id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
          employee = new Employee(reader.GetInt32(reader.GetOrdinal("Id")), reader.GetString(reader.GetOrdinal("Name")));
        }
        else
        {
          throw new RecordNotFoundException("No employee found with the given ID.");
        }
      }

      return employee!;

    }
    // TODO: validate variable does nothing malicious with the query
    private const string queryById = "SELECT Id, Name FROM Employee WHERE Id = @id;";
  }

}