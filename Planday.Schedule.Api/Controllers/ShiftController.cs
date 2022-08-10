using System.Data.SQLite;
using Microsoft.AspNetCore.Mvc;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftController : ControllerBase
    {
        readonly IGetAllShiftsQuery _getAllShiftsQuery;
        readonly IPostOpenShiftQuery _postOpenShiftQuery;
        public ShiftController(
            IGetAllShiftsQuery getAllShiftsQuery, 
            IPostOpenShiftQuery postOpenShiftQuery)
        {
            _getAllShiftsQuery = getAllShiftsQuery;
            _postOpenShiftQuery = postOpenShiftQuery;
        }

        [HttpGet]
        public IActionResult Get(long shiftId) 
        {
            var shift = Task.Run(() => _getAllShiftsQuery.QueryAsync()).Result.FirstOrDefault(s => s.Id == shiftId);
            return shift is null ? NotFound() : Ok(shift);
        }

        [HttpPost]
        public IActionResult PostOpenShift(Shift shift)
        {
            if (shift.Start.CompareTo(shift.End) > 0) return BadRequest("The start time must not be greater than the end time");
            if (shift.Start.Date != shift.End.Date) return BadRequest("Start and end time should be in the same day");
            try
            {
                Task.Run(() => _postOpenShiftQuery.ExecuteAsync(shift));
                return Ok("Shift created successfully");
            }
            catch (SQLiteException e)
            {
                return Conflict(e.Message);
            }
        }
    }    
}

