using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class PostOpenShiftQuery : IPostOpenShiftQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public PostOpenShiftQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }
    
        public async void ExecuteAsync(Shift shift)
        {
            await using var sqlConnection = new SQLiteConnection(_connectionStringProvider.GetConnectiongString());

            try
            {
                var sqlString = string.Format(
                    "INSERT INTO Shift (EmployeeId, Start, End) VALUES (NULL, '{0:yyyy-MM-dd hh:mm:ss.fff}', '{1:yyyy-MM-dd hh:mm:ss.fff}');",
                    shift.Start, shift.End);
                await sqlConnection.ExecuteAsync(sqlString);
            }
            catch(SQLiteException e)
            {
                throw e;
            }
        }
    
        private record ShiftDto(long Id, long? EmployeeId, string Start, string End);
    }    
}

