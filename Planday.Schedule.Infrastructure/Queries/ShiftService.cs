using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Exceptions;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class ShiftService : IShiftService
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IEmployeeService _employeeService;

        public ShiftService(IConnectionStringProvider connectionStringProvider, IEmployeeService employeeService)
        {
            _connectionStringProvider = connectionStringProvider;
            _employeeService = employeeService;
        }

        public async Task<IReadOnlyCollection<Shift>> GetAllShifts()
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(queryAll);

            var shifts = shiftDtos.Select(x =>
                new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

            return shifts.ToList();
        }

        private const string queryAll = @"SELECT Id, EmployeeId, Start, End FROM Shift;";

        private record ShiftDto(long Id, long? EmployeeId, string Start, string End);

        public async Task<Shift> GetShiftById(long id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            sqlConnection.Open();

            Shift? shift = null;
            using (var command = new SqliteCommand(queryById, sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    shift = new Shift(reader.GetInt32(reader.GetOrdinal("Id")), reader.GetInt32(reader.GetOrdinal("EmployeeId")), DateTime.Parse(reader.GetString(reader.GetOrdinal("Start"))), DateTime.Parse(reader.GetString(reader.GetOrdinal("End"))));
                }
                else
                {
                    throw new RecordNotFoundException("No shift found with the given ID.");
                }
            }

            return shift!;

        }

        // TODO: validate variable does nothing malicious with the query
        private const string queryById = "SELECT Id, EmployeeId, Start, End FROM Shift WHERE Id = @id;";

        public async Task<Shift> CreateShift(string start, string end)
        {
            int newId;
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            sqlConnection.Open();

            // Get the highest current ID to increment
            string getMaxIdQuery = "SELECT IFNULL(MAX(Id), 0) FROM Shift;";
            using (var command = new SqliteCommand(getMaxIdQuery, sqlConnection))
            {
                newId = Convert.ToInt32(command.ExecuteScalar()) + 1;
            }

            // Insert the new record
            string insertQuery = "INSERT INTO Shift (Id, Start, End) VALUES (@Id, @Start, @End);";
            using (var command = new SqliteCommand(insertQuery, sqlConnection))
            {
                command.Parameters.AddWithValue("@Id", newId);
                command.Parameters.AddWithValue("@Start", start);
                command.Parameters.AddWithValue("@End", end);
                command.ExecuteNonQuery();
            }

            return new Shift(newId, null, DateTime.Parse(start), DateTime.Parse(end));
        }

        public async Task<Shift> AssignEmployeeToShift(long shiftId, long employeeId)
        {
            // TODO: handle employeeId not returning a row
            using var employee = _employeeService.GetEmployeeById(employeeId);

            // TODO: handle shiftId not returning a row
            using var shift = GetShiftById(shiftId);

            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            sqlConnection.Open();

            // Update employeedId in shift
            using var command = new SqliteCommand(updateEmplyeeId, sqlConnection);
            command.Parameters.AddWithValue("@Id", shiftId);
            command.Parameters.AddWithValue("@EmployeedId", employeeId);
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                throw new RecordNotFoundException($"No record found with ID {shiftId}");
            }

            var updatedShift = await GetShiftById(shiftId);
            return updatedShift;
        }
        // TODO: validate variables does nothing malicious with the query
        private const string updateEmplyeeId = "UPDATE Shift SET EmployeeId = @EmployeedId WHERE Id = @Id;";
    }
}

