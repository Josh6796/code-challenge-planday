using System.Collections.Generic;

namespace Planday.Schedule.Tests.MockData
{
    class EmployeeMockData
    {
        public static List<Employee> GetEmployees()
        {
            return new List<Employee>{
                new (1, "John Doe"),
                new (2,  "Jane Doe")
            };
        }
    }
}
