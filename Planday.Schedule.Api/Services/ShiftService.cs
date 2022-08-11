using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Api.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IGetAllShiftsQuery _getAllShiftsQuery;
        private readonly IGetAllEmployeesQuery _getAllEmployeesQuery;
        private readonly IPostOpenShiftQuery _postOpenShiftQuery;
        private readonly IUpdateShiftQuery _updateShiftQuery;

        public ShiftService(
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

        public async Task<IReadOnlyCollection<Shift>> getAllShifts()
        {
            return await _getAllShiftsQuery.QueryAsync();
        }

        public async Task<IReadOnlyCollection<Employee>> getAllEmployees()
        {
            return await _getAllEmployeesQuery.QueryAsync();
        }

        public void postOpenShift(Shift shift)
        {
            _postOpenShiftQuery.ExecuteAsync(shift);
        }

        public void updateShift(long shiftId, long employeeId)
        {
            _updateShiftQuery.ExecuteAsync(shiftId, employeeId);
        }
    }
}
