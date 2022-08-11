using System.Data.SQLite;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Planday.Schedule.Api.Services;

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
        [Route("/getShiftById")]
        public async Task<IActionResult> GetShiftById(long shiftId)
        {
            var shifts = await _shiftService.getAllShifts();
            var shift = shifts.FirstOrDefault(s => s.Id == shiftId);

            if (shift is null) return NotFound();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "8e0ac353-5ef1-4128-9687-fb9eb8647288");
            var response = await client.GetAsync($"http://20.101.230.231:5000/employee/{shift?.EmployeeId}");
            var employeeDto = JsonConvert.DeserializeObject<EmployeeDto>(await response.Content.ReadAsStringAsync());

            return employeeDto is null ? Ok(shift) : Ok(new ShiftEmployeeDto(shift.Id, shift.EmployeeId, shift.Start, shift.End, employeeDto.Email));
        }

        [HttpPost]
        [Route("/createOpenShift")]
        public IActionResult CreateOpenShift(Shift shift)
        {
            if (shift.Start > shift.End) return BadRequest("The start time must not be greater than the end time");
            if (shift.Start.Date != shift.End.Date) return BadRequest("Start and end time should be in the same day");
            try
            {
                _shiftService.postOpenShift(shift);
                return Ok("Shift created successfully");
            }
            catch (SQLiteException e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPost]
        [Route("/assignShiftToEmployee")]
        public async Task<IActionResult> AssignShiftToEmployee(long shiftId, long employeeId)
        {
            var employees = await _shiftService.getAllEmployees();
            var employee = employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee is null) return BadRequest($"There is no employee with ID {employeeId}");
            var shifts = await _shiftService.getAllShifts();
            var shift = shifts.FirstOrDefault(s => s.Id == shiftId);
            if (shift is null) return BadRequest($"There is no shift with ID {shiftId}");
            
            foreach (var s in shifts)
            {
                if (s.EmployeeId != employeeId)
                {
                    if (shift.EmployeeId is not null && s.EmployeeId == shift.EmployeeId)
                        return BadRequest("You cannot assign the same shift to two or more employees");
                    continue;
                }
                if (s.Id == shiftId) return BadRequest($"Employee {employee.Name} has already assigned this Shift");
                if (shift.Start < s.End && s.Start < shift.End) 
                    return BadRequest("Shifts are overlapping");
            }

            try
            {
                _shiftService.updateShift(shiftId, employeeId);
                return Ok("Shift assigned successfully");
            }
            catch (SQLiteException e)
            {
                return Conflict(e.Message);
            }
        }

        private record EmployeeDto(string Name, string Email);

        private record ShiftEmployeeDto(long Id, long? EmployeeId, DateTime Start, DateTime End, string Email);
    }    
}

