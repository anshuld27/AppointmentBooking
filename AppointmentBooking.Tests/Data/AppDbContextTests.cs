using AppointmentBooking.Data;
using AppointmentBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentBooking.Tests.Data
{
    public class AppDbContextTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public AppDbContextTests()
        {
            // Configure in-memory database with unique name for each test run
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Prevent test collisions
                .Options;
        }

        [Fact]
        public void CanAddAndRetrieveSalesManager()
        {
            // Arrange - Create context and test data
            using var context = new AppDbContext(_dbContextOptions);
            var salesManager = new SalesManager
            {
                Id = 1,
                Name = "John Doe",
                Languages = new List<string> { "English" },
                Products = new List<string> { "Product1" },
                CustomerRatings = new List<string> { "5" }
            };

            // Act - Add entity and save changes
            context.SalesManagers.Add(salesManager);
            context.SaveChanges();

            // Retrieve the entity from database
            var retrievedSalesManager = context.SalesManagers.Find(1);

            // Assert - Verify data persistence
            Assert.NotNull(retrievedSalesManager);
            Assert.Equal("John Doe", retrievedSalesManager.Name);
        }

        [Fact]
        public void CanAddAndRetrieveSlot()
        {
            // Arrange - Create related entities
            using var context = new AppDbContext(_dbContextOptions);

            // Create required SalesManager first due to foreign key relationship
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
                SalesManager = salesManager  // Establishing relationship
            };

            // Act - Add both entities and save
            context.SalesManagers.Add(salesManager);
            context.Slots.Add(slot);
            context.SaveChanges();

            var retrievedSlot = context.Slots.Find(1);

            // Assert - Verify slot persistence and time values
            Assert.NotNull(retrievedSlot);
            Assert.Equal(slot.StartDate, retrievedSlot.StartDate);
        }

        [Fact]
        public void OnModelCreating_ConfiguresSalesManagerEntity()
        {
            // Arrange - Get access to EF Core's model metadata
            using var context = new AppDbContext(_dbContextOptions);
            var model = context.Model;

            // Act - Retrieve entity type configuration
            var salesManagerEntity = model.FindEntityType(typeof(SalesManager));

            // Assert - Verify table name and property configurations
            Assert.NotNull(salesManagerEntity);
            Assert.Equal("sales_managers", salesManagerEntity.GetTableName()); // Check table name mapping

            // Verify complex properties are properly mapped
            Assert.Equal("Languages", salesManagerEntity?.FindProperty(nameof(SalesManager.Languages))?.Name);
            Assert.Equal("Products", salesManagerEntity?.FindProperty(nameof(SalesManager.Products))?.Name);
            Assert.Equal("CustomerRatings", salesManagerEntity?.FindProperty(nameof(SalesManager.CustomerRatings))?.Name);
        }

        [Fact]
        public void OnModelCreating_ConfiguresSlotEntity()
        {
            // Arrange
            using var context = new AppDbContext(_dbContextOptions);
            var model = context.Model;

            // Act
            var slotEntity = model.FindEntityType(typeof(Slot));

            // Assert
            Assert.NotNull(slotEntity);
            var slotEntityNonNull = slotEntity!; // Tell compiler it's non-null after assertion

            // Table configuration
            Assert.Equal("slots", slotEntityNonNull.GetTableName());

            // Property configuration
            Assert.Equal("StartDate", slotEntityNonNull.FindProperty(nameof(Slot.StartDate))!.Name);
            Assert.Equal("EndDate", slotEntityNonNull.FindProperty(nameof(Slot.EndDate))!.Name);

            // Index configuration
            var indexes = slotEntityNonNull.GetIndexes();
            Assert.Contains(indexes, i => i.Properties.Any(p => p.Name == nameof(Slot.SalesManagerId)));
            Assert.Contains(indexes, i => i.Properties.Any(p => p.Name == nameof(Slot.StartDate)));
            Assert.Contains(indexes, i => i.Properties.Any(p => p.Name == nameof(Slot.EndDate)));
        }
    }
}