

namespace Planday.Schedule.Queries
{
    public interface IGetAllEmployeesQuery
    {
        Task<IReadOnlyCollection<Employee>> QueryAsync();
    }
}