using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Planday.Schedule.Infrastructure.Exceptions;
using Planday.Schedule.Infrastructure.Queries;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;
        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Shift>>> GetAllShifts()
        {
            var shifts = await _shiftService.GetAllShifts();
            return Ok(shifts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shift>> GetShift(long id)
        {
            var shift = await _shiftService.GetShift(id);
            return Ok(shift);
        }

        [HttpPost]
        public async Task<ActionResult<Shift>> Create([FromBody] DateRange dateRange)
        {
            if (string.IsNullOrWhiteSpace(dateRange.Start) || string.IsNullOrWhiteSpace(dateRange.End))
            {
                return BadRequest("Start date and end date are required.");
            }

            // Ensure start is not greater than end time
            DateTime DateStart = DateTime.Parse(dateRange.Start);
            DateTime DateEnd = DateTime.Parse(dateRange.End);
            if (DateStart > DateEnd)
            {
                return BadRequest("Start date needs to be before the end date.");
            }
            // Check that dates are in the same day
            if (DateStart.Date != DateEnd.Date)
            {
                return BadRequest("Start and end dates need to be in the same day.");
            }

            var newShift = await _shiftService.Create(dateRange.Start, dateRange.End);
            return CreatedAtAction(nameof(GetShift), new { id = newShift.Id }, newShift);
        }

        [HttpPut("{id}/{employeeId}")]
        public async Task<ActionResult<Shift>> AssignEmployee(int id, int employeeId)
        {
            var updatedShift = await _shiftService.AssignEmployee(id, employeeId);
            return Ok(updatedShift);
        }
    }

}

public class DateRange
{
    public string? Start { get; set; }
    public string? End { get; set; }
}