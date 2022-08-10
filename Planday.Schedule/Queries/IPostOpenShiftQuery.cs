using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries
{
    public interface IPostOpenShiftQuery
    {
        void ExecuteAsync(Shift shift);
    }    
}

