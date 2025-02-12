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
    }
}

