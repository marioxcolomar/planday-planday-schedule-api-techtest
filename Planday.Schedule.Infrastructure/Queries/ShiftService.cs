using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class ShiftService : IShiftService
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public ShiftService(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
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

            Shift shift = null;
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
                    Console.WriteLine("No record found with the given ID.");

                }
            }

            return shift!;

        }

        private const string queryById = "SELECT Id, EmployeeId, Start, End FROM Shift WHERE Id = @id;";
    }
}

