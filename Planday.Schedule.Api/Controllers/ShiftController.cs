using System.Data.SQLite;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftController : ControllerBase
    {
        private readonly IGetAllShiftsQuery _getAllShiftsQuery;
        private readonly IGetAllEmployeesQuery _getAllEmployeesQuery;
        private readonly IPostOpenShiftQuery _postOpenShiftQuery;
        private readonly IUpdateShiftQuery _updateShiftQuery;
        public ShiftController(
            IGetAllShiftsQuery getAllShiftsQuery,
            IGetAllEmployeesQuery getAllEmployeesQuery,
            IPostOpenShiftQuery postOpenShiftQuery,
            IUpdateShiftQuery updateShiftQuery)
        {
            _getAllShiftsQuery = getAllShiftsQuery;
            _getAllEmployeesQuery = getAllEmployeesQuery;
            _postOpenShiftQuery = postOpenShiftQuery;
            _updateShiftQuery = updateShiftQuery;
        }

        [HttpGet]
        [Route("/getShiftById")]
        public IActionResult GetShiftById(long shiftId) 
        {
            var shift = _getAllShiftsQuery.QueryAsync().Result.FirstOrDefault(s => s.Id == shiftId);

            if (shift is null) return NotFound();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "8e0ac353-5ef1-4128-9687-fb9eb8647288");
            var response = client.GetAsync($"http://20.101.230.231:5000/employee/{shift?.EmployeeId}").Result;
            var employeeDto = JsonConvert.DeserializeObject<EmployeeDto>(response.Content.ReadAsStringAsync().Result);

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
                _postOpenShiftQuery.ExecuteAsync(shift);
                return Ok("Shift created successfully");
            }
            catch (SQLiteException e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPost]
        [Route("/assignShiftToEmployee")]
        public IActionResult AssignShiftToEmployee(long shiftId, long employeeId)
        {
            var employee = Task.Run(() => _getAllEmployeesQuery.QueryAsync()).Result.FirstOrDefault(e => e.Id == employeeId);
            if (employee is null) return BadRequest($"There is no employee with ID {employeeId}");
            var shift = Task.Run(() => _getAllShiftsQuery.QueryAsync()).Result.FirstOrDefault(s => s.Id == shiftId);
            if (shift is null) return BadRequest($"There is no shift with ID {shiftId}");

            var shifts = Task.Run(() => _getAllShiftsQuery.QueryAsync()).Result;
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
                    return BadRequest($"Shifts are overlapping");
            }

            try
            {
                _updateShiftQuery.ExecuteAsync(shiftId, employeeId);
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

