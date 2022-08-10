using System.Data.SQLite;
using Dapper;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class GetAllEmployeesQuery : IGetAllEmployeesQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public GetAllEmployeesQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }
    
        public async Task<IReadOnlyCollection<Employee>> QueryAsync()
        {
            await using var sqlConnection = new SQLiteConnection(_connectionStringProvider.GetConnectiongString());

            var employeeDtos = await sqlConnection.QueryAsync<EmployeeDto>(Sql);

            var employees = employeeDtos.Select(x => 
                new Employee(x.Id, x.Name));
        
            return employees.ToList();
        }

        private const string Sql = @"SELECT Id, Name FROM Employee;";
    
        private record EmployeeDto(long Id, string Name);
    }    
}

