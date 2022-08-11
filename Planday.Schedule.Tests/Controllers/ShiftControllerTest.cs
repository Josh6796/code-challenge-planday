using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Planday.Schedule.Api.Controllers;
using Planday.Schedule.Api.Services;
using Planday.Schedule.Tests.MockData;
using Xunit;

namespace Planday.Schedule.Tests.Controllers
{
    public class ShiftControllerTest
    {
        [Fact]
        public async Task GetShiftById_ShouldReturn200Status()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShifts);
            var sut = new ShiftController(shiftServiceMock.Object);

            /// Act
            var result = (OkObjectResult)await sut.GetShiftById(1);


            // /// Assert
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetShiftById_ShouldReturn404Status()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShifts);
            var sut = new ShiftController(shiftServiceMock.Object);

            /// Act
            var result = (NotFoundResult)await sut.GetShiftById(4);

            // /// Assert
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void CreateOpenShift_ShouldReturn200Status()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShifts);
            var sut = new ShiftController(shiftServiceMock.Object);
            var shift = new Shift(3, null, DateTime.Now, DateTime.Now.AddHours(2));

            /// Act
            var result = (OkObjectResult) sut.CreateOpenShift(shift);

            // /// Assert
            result.StatusCode.Should().Be(200);
            result.Value.Should().Be("Shift created successfully");
        }

        [Fact]
        public void CreateOpenShift_ShouldReturn400Status_StartTimeGreaterThanEndTime()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShifts);
            var sut = new ShiftController(shiftServiceMock.Object);
            var shift = new Shift(3, null, DateTime.Now.AddHours(2), DateTime.Now);

            /// Act
            var result = (BadRequestObjectResult)sut.CreateOpenShift(shift);

            // /// Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("The start time must not be greater than the end time");
        }

        [Fact]
        public void CreateOpenShift_ShouldReturn400Status_StartAndEndShouldBeSameDay()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShifts);
            var sut = new ShiftController(shiftServiceMock.Object);
            var shift = new Shift(3, null, DateTime.Now, DateTime.Now.AddDays(2));

            /// Act
            var result = (BadRequestObjectResult)sut.CreateOpenShift(shift);

            // /// Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("Start and end time should be in the same day");
        }

        [Fact]
        public async Task AssignShiftToEmployee_ShouldReturn200Status()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            var shift = new Shift(3, null, DateTime.Now, DateTime.Now.AddHours(2));
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShiftsWithOpenShift);
            shiftServiceMock.Setup(_ => _.getAllEmployees()).ReturnsAsync(EmployeeMockData.GetEmployees);
            var sut = new ShiftController(shiftServiceMock.Object);

            /// Act
            var result = (OkObjectResult)await sut.AssignShiftToEmployee(shift.Id, 2);

            // /// Assert
            result.StatusCode.Should().Be(200);
            result.Value.Should().Be("Shift assigned successfully");
        }

        [Fact]
        public async Task AssignShiftToEmployee_ShouldReturn400Status_AlreadyAssigned()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShifts);
            shiftServiceMock.Setup(_ => _.getAllEmployees()).ReturnsAsync(EmployeeMockData.GetEmployees);
            var sut = new ShiftController(shiftServiceMock.Object);

            /// Act
            var result = (BadRequestObjectResult)await sut.AssignShiftToEmployee(2, 2);

            // /// Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("Employee Jane Doe has already assigned this Shift");
        }

        [Fact]
        public async Task AssignShiftToEmployee_ShouldReturn400Status_NoSuchShift()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShifts);
            shiftServiceMock.Setup(_ => _.getAllEmployees()).ReturnsAsync(EmployeeMockData.GetEmployees);
            var sut = new ShiftController(shiftServiceMock.Object);

            /// Act
            var result = (BadRequestObjectResult)await sut.AssignShiftToEmployee(3, 2);

            // /// Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("There is no shift with ID 3");
        }

        [Fact]
        public async Task AssignShiftToEmployee_ShouldReturn400Status_NoSuchEmployee()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShifts);
            shiftServiceMock.Setup(_ => _.getAllEmployees()).ReturnsAsync(EmployeeMockData.GetEmployees);
            var sut = new ShiftController(shiftServiceMock.Object);

            /// Act
            var result = (BadRequestObjectResult)await sut.AssignShiftToEmployee(2, 3);

            // /// Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("There is no employee with ID 3");
        }

        [Fact]
        public async Task AssignShiftToEmployee_ShouldReturn400Status_OverlappingShifts()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShiftsWithOpenShiftOverlapping);
            shiftServiceMock.Setup(_ => _.getAllEmployees()).ReturnsAsync(EmployeeMockData.GetEmployees);
            var sut = new ShiftController(shiftServiceMock.Object);

            /// Act
            var result = (BadRequestObjectResult)await sut.AssignShiftToEmployee(3, 2);

            // /// Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("Shifts are overlapping");
        }

        [Fact]
        public async Task AssignShiftToEmployee_ShouldReturn400Status_CannotAssignToMoreEmployees()
        {
            /// Arrange
            var shiftServiceMock = new Mock<IShiftService>();
            shiftServiceMock.Setup(_ => _.getAllShifts()).ReturnsAsync(ShiftMockData.GetShiftsWithThirdAssignedShift);
            shiftServiceMock.Setup(_ => _.getAllEmployees()).ReturnsAsync(EmployeeMockData.GetEmployees);
            var sut = new ShiftController(shiftServiceMock.Object);

            /// Act
            var result = (BadRequestObjectResult)await sut.AssignShiftToEmployee(3, 1);

            // /// Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("You cannot assign the same shift to two or more employees");
        }
    }
}
