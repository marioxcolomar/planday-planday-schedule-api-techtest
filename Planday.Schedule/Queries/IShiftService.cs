using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries
{
    public interface IShiftService
    {
        Task<IReadOnlyCollection<Shift>> GetAllShifts();
        Task<Shift> GetShift(long Id);
        Task<Shift> Create(string start, string end);
        Task<Shift> AssignEmployee(long shiftId, long employeeId);
    }
}

