using AppointmentBooking.Data;
using AppointmentBooking.DTOs;
using AppointmentBooking.Models;
using AppointmentBooking.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppointmentBooking.Tests.Services
{
    public class CalendarServiceTests
    {
        private readonly Mock<ILogger<CalendarService>> _loggerMock;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public CalendarServiceTests()
        {
            // Initialize mock logger and in-memory database options for testing
            _loggerMock = new Mock<ILogger<CalendarService>>();
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for isolated testing
                .Options;
        }

        // Helper method to clear database entities between tests
        private async Task ClearDatabase(AppDbContext context)
        {
            context.SalesManagers.RemoveRange(context.SalesManagers);
            context.Slots.RemoveRange(context.Slots);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_ReturnsAvailableSlots()
        {
            // Arrange - Setup test data
            using var context = new AppDbContext(_dbContextOptions);
            await ClearDatabase(context);
            var service = new CalendarService(context, _loggerMock.Object);

            // Create matching sales manager and available slot
            var salesManager = new SalesManager
            {
                Id = 1,
                Name = "John Doe",
                Languages = new List<string> { "English" },
                Products = new List<string> { "Product1" },
                CustomerRatings = new List<string> { "5" }
            };

            var slot = new Slot
            {
                Id = 1,
                StartDate = DateTime.UtcNow.Date.AddHours(9),
                EndDate = DateTime.UtcNow.Date.AddHours(10),
                Booked = false,
                SalesManagerId = 1,
                SalesManager = salesManager
            };

            context.SalesManagers.Add(salesManager);
            context.Slots.Add(slot);
            await context.SaveChangesAsync();

            var request = new CalendarQueryRequest
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Language = "English",
                Products = new List<string> { "Product1" },
                Rating = "5"
            };

            // Act - Execute the service method
            var result = await service.GetAvailableSlotsAsync(request);

            // Assert - Verify correct available slot is returned
            Assert.Single(result);
            Assert.Equal(slot.StartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), result.First().StartDate);
            Assert.Equal(1, result.First().AvailableCount);
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_ThrowsException_LogsError()
        {
            // Arrange - Setup invalid request data
            using var context = new AppDbContext(_dbContextOptions);
            await ClearDatabase(context);
            var service = new CalendarService(context, _loggerMock.Object);

            var request = new CalendarQueryRequest
            {
                Date = "invalid-date", // Invalid date format
                Language = "English",
                Products = new List<string> { "Product1" },
                Rating = "5"
            };

            // Act & Assert - Verify exception is thrown and logged
            var exception = await Assert.ThrowsAsync<FormatException>(() => service.GetAvailableSlotsAsync(request));
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_ReturnsEmpty_WhenNoProducts()
        {
            // Arrange - Request with empty products list
            using var context = new AppDbContext(_dbContextOptions);
            await ClearDatabase(context);
            var service = new CalendarService(context, _loggerMock.Object);

            var request = new CalendarQueryRequest
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Language = "English",
                Products = new List<string>(), // No products specified
                Rating = "5"
            };

            // Act
            var result = await service.GetAvailableSlotsAsync(request);

            // Assert - Should return empty when no products requested
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_ReturnsEmpty_WhenInvalidLanguage()
        {
            // Arrange - Setup data with mismatched language
            using var context = new AppDbContext(_dbContextOptions);
            await ClearDatabase(context);
            var service = new CalendarService(context, _loggerMock.Object);

            var salesManager = new SalesManager
            {
                Id = 1,
                Name = "John Doe",
                Languages = new List<string> { "English" }, // Manager only knows English
                Products = new List<string> { "Product1" },
                CustomerRatings = new List<string> { "5" }
            };

            var slot = new Slot
            {
                Id = 1,
                StartDate = DateTime.UtcNow.Date.AddHours(9),
                EndDate = DateTime.UtcNow.Date.AddHours(10),
                Booked = false,
                SalesManagerId = 1,
                SalesManager = salesManager
            };

            context.SalesManagers.Add(salesManager);
            context.Slots.Add(slot);
            await context.SaveChangesAsync();

            var request = new CalendarQueryRequest
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Language = "Spanish", // Requesting Spanish which manager doesn't know
                Products = new List<string> { "Product1" },
                Rating = "5"
            };

            // Act
            var result = await service.GetAvailableSlotsAsync(request);

            // Assert - No slots should match language requirement
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_ReturnsEmpty_WhenNoAvailableSlots()
        {
            // Arrange - Empty database
            using var context = new AppDbContext(_dbContextOptions);
            await ClearDatabase(context);
            var service = new CalendarService(context, _loggerMock.Object);

            var request = new CalendarQueryRequest
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Language = "English",
                Products = new List<string> { "Product1" },
                Rating = "5"
            };

            // Act
            var result = await service.GetAvailableSlotsAsync(request);

            // Assert - No slots exist in system
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_ReturnsEmpty_WhenAllSlotsBooked()
        {
            // Arrange - Setup booked slot
            using var context = new AppDbContext(_dbContextOptions);
            await ClearDatabase(context);
            var service = new CalendarService(context, _loggerMock.Object);

            var salesManager = new SalesManager
            {
                Id = 1,
                Name = "John Doe",
                Languages = new List<string> { "English" },
                Products = new List<string> { "Product1" },
                CustomerRatings = new List<string> { "5" }
            };

            var slot = new Slot
            {
                Id = 1,
                StartDate = DateTime.UtcNow.Date.AddHours(9),
                EndDate = DateTime.UtcNow.Date.AddHours(10),
                Booked = true, // Marked as booked
                SalesManagerId = 1,
                SalesManager = salesManager
            };

            context.SalesManagers.Add(salesManager);
            context.Slots.Add(slot);
            await context.SaveChangesAsync();

            var request = new CalendarQueryRequest
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Language = "English",
                Products = new List<string> { "Product1" },
                Rating = "5"
            };

            // Act
            var result = await service.GetAvailableSlotsAsync(request);

            // Assert - No available slots should be returned
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_ReturnsMultipleSlots_WhenMultipleAvailable()
        {
            // Arrange - Setup multiple available slots
            using var context = new AppDbContext(_dbContextOptions);
            await ClearDatabase(context);
            var service = new CalendarService(context, _loggerMock.Object);

            var salesManager = new SalesManager
            {
                Id = 1,
                Name = "John Doe",
                Languages = new List<string> { "English" },
                Products = new List<string> { "Product1" },
                CustomerRatings = new List<string> { "5" }
            };

            // Create two available slots
            var slot1 = new Slot
            {
                Id = 1,
                StartDate = DateTime.UtcNow.Date.AddHours(9),
                EndDate = DateTime.UtcNow.Date.AddHours(10),
                Booked = false,
                SalesManagerId = 1,
                SalesManager = salesManager
            };

            var slot2 = new Slot
            {
                Id = 2,
                StartDate = DateTime.UtcNow.Date.AddHours(10),
                EndDate = DateTime.UtcNow.Date.AddHours(11),
                Booked = false,
                SalesManagerId = 1,
                SalesManager = salesManager
            };

            context.SalesManagers.Add(salesManager);
            context.Slots.AddRange(slot1, slot2);
            await context.SaveChangesAsync();

            var request = new CalendarQueryRequest
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Language = "English",
                Products = new List<string> { "Product1" },
                Rating = "5"
            };

            // Act
            var result = await service.GetAvailableSlotsAsync(request);

            // Assert - Both available slots should be returned
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_ThrowsException_WhenRequestIsNull()
        {
            // Arrange
            using var context = new AppDbContext(_dbContextOptions);
            await ClearDatabase(context);
            var service = new CalendarService(context, _loggerMock.Object);

            // Act & Assert - Verify null request throws ArgumentNullException
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetAvailableSlotsAsync(null!));
        }
    }
}
