using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries
{
    public interface IShiftService
    {
        Task<IReadOnlyCollection<Shift>> GetAllShifts();
        Task<Shift> GetShiftById(long Id);
        Task<Shift> CreateShift(string start, string end);
        Task<Shift> AssignEmployeeToShift(long shiftId, long employeeId);
    }
}

