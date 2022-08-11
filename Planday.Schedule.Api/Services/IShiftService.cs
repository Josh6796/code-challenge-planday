namespace Planday.Schedule.Api.Services
{
    public interface IShiftService
    {
        Task<IReadOnlyCollection<Shift>> getAllShifts();
        Task<IReadOnlyCollection<Employee>> getAllEmployees();
        void postOpenShift(Shift shift);
        void updateShift(long shiftId, long employeeId);

    }
}
