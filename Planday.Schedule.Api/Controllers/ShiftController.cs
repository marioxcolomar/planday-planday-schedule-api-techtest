using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
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
        public Task<IReadOnlyCollection<Shift>> GetAllShifts()
        {
            return _shiftService.GetAllShifts();
        }

        [HttpGet("{id}")]
        public ActionResult<Shift> GetShiftById(long id)
        {
            var shift = _shiftService.GetShiftById(id);
            if (shift == null)
            {
                return NotFound("Record not found.");
            }
            return Ok(shift.Result);
        }

        [HttpPost]
        public ActionResult<Shift> CreateShift([FromBody] DateRange dateRange)
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

            var newShift = _shiftService.CreateShift(dateRange.Start, dateRange.End);
            return CreatedAtAction(nameof(GetShiftById), new { id = newShift.Result.Id }, newShift.Result);
        }
    }

}

public class DateRange
{
    public string? Start { get; set; }
    public string? End { get; set; }
}