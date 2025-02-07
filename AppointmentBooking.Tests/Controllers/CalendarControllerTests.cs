using AppointmentBooking.Controllers;
using AppointmentBooking.DTOs;
using AppointmentBooking.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AppointmentBooking.Tests.Controllers
{
    // Test class for CalendarController containing unit tests
    public class CalendarControllerTests
    {
        // Mock of the calendar service to simulate dependencies
        private readonly Mock<ICalendarService> _calendarServiceMock;
        // Instance of the controller being tested, initialized with mock service
        private readonly CalendarController _controller;

        // Constructor initializes common test dependencies before each test
        public CalendarControllerTests()
        {
            // Create mock instance of ICalendarService
            _calendarServiceMock = new Mock<ICalendarService>();
            // Instantiate controller with mocked service (dependency injection)
            _controller = new CalendarController(_calendarServiceMock.Object);
        }

        // Test case: Verify controller returns OK result with available slots when valid request is made
        [Fact]
        public async Task GetAvailableSlots_ReturnsOkResult_WithAvailableSlots()
        {
            // Arrange - set up test data and dependencies
            var request = new CalendarQueryRequest
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"), // Valid date format
                Language = "English",
                Products = new List<string> { "Product1" },
                Rating = "5"
            };

            // Mock service response
            var availableSlots = new List<AvailableSlotResponse>
            {
                new AvailableSlotResponse(DateTime.UtcNow.Date.AddHours(9), 1)
            };

            // Setup mock service to return expected slots when called with test request
            _calendarServiceMock.Setup(service => service.GetAvailableSlotsAsync(request))
                .ReturnsAsync(availableSlots);

            // Act - call the controller method
            var result = await _controller.QueryAvailableSlots(request);

            // Assert - verify the results
            var okResult = Assert.IsType<OkObjectResult>(result); // Check response is OkObjectResult
            var returnValue = Assert.IsType<List<AvailableSlotResponse>>(okResult.Value); // Verify response type
            Assert.Single(returnValue); // Confirm exactly one slot returned
            Assert.Equal(availableSlots, returnValue); // Verify returned slots match mock response
        }

        // Test case: Verify controller returns BadRequest when invalid date format is provided
        [Fact]
        public async Task GetAvailableSlots_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange - create request with invalid date format
            var request = new CalendarQueryRequest
            {
                Date = "invalid-date", // Invalid date format to trigger exception
                Language = "English",
                Products = new List<string> { "Product1" },
                Rating = "5"
            };

            // Setup mock service to throw exception when called with invalid request
            _calendarServiceMock.Setup(service => service.GetAvailableSlotsAsync(request))
                .ThrowsAsync(new FormatException());

            // Act - call controller method
            var result = await _controller.QueryAvailableSlots(request);

            // Assert - verify error response
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); // Check for BadRequest result
            Assert.Equal("Invalid date format.", badRequestResult.Value); // Verify error message
        }
    }
}