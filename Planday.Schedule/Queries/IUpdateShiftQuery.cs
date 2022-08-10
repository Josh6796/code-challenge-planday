namespace Planday.Schedule.Queries
{
    public interface IUpdateShiftQuery
    {
        void ExecuteAsync(long shiftId, long employeeId);
    }    
}

