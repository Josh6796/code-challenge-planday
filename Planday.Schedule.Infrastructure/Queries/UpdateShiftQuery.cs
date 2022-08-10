using System.Data.SQLite;
using Dapper;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class UpdateShiftQuery : IUpdateShiftQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public UpdateShiftQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }
    
        public async void ExecuteAsync(long shiftId, long employeeId)
        {
            await using var sqlConnection = new SQLiteConnection(_connectionStringProvider.GetConnectiongString());

            try
            {
                var sqlString = string.Format(
                    "UPDATE Shift SET EmployeeId = {0} WHERE ID = {1};",
                    employeeId, shiftId);
                await sqlConnection.ExecuteAsync(sqlString);
            }
            catch(SQLiteException e)
            {
                throw e;
            }
        }
    }    
}

