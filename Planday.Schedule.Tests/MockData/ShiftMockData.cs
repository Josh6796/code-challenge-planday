using System;
using System.Collections.Generic;

namespace Planday.Schedule.Tests.MockData
{
    class ShiftMockData
    {
        public static List<Shift> GetShifts()
        {
            return new List<Shift>{
                new (1, 1, DateTime.Parse("2022-06-17 12:00:00.000"), DateTime.Parse("2022-06-17 17:00:00.000")),
                new (2,  2, DateTime.Parse("2022-06-17 09:00:00.00"), DateTime.Parse("2022-06-17 15:00:00.000"))
            };
        }
        public static List<Shift> GetShiftsWithOpenShift()
        {
            return new List<Shift>{
                new (1, 1, DateTime.Parse("2022-06-17 12:00:00.000"), DateTime.Parse("2022-06-17 17:00:00.000")),
                new (2,  2, DateTime.Parse("2022-06-17 09:00:00.00"), DateTime.Parse("2022-06-17 15:00:00.000")),
                new (3,  null, DateTime.Parse("2022-06-18 15:00:00.00"), DateTime.Parse("2022-06-18 18:00:00.000"))
            };
        }
        public static List<Shift> GetShiftsWithOpenShiftOverlapping()
        {
            return new List<Shift>{
                new (1, 1, DateTime.Parse("2022-06-17 12:00:00.000"), DateTime.Parse("2022-06-17 17:00:00.000")),
                new (2,  2, DateTime.Parse("2022-06-17 09:00:00.00"), DateTime.Parse("2022-06-17 15:00:00.000")),
                new (3,  null, DateTime.Parse("2022-06-17 13:00:00.00"), DateTime.Parse("2022-06-17 18:00:00.000"))
            };
        }
        public static List<Shift> GetShiftsWithThirdAssignedShift()
        {
            return new List<Shift>{
                new (1, 1, DateTime.Parse("2022-06-17 12:00:00.000"), DateTime.Parse("2022-06-17 17:00:00.000")),
                new (2,  2, DateTime.Parse("2022-06-17 09:00:00.00"), DateTime.Parse("2022-06-17 15:00:00.000")),
                new (3,  2, DateTime.Parse("2022-06-18 13:00:00.00"), DateTime.Parse("2022-06-18 18:00:00.000"))
            };
        }
    }
}
