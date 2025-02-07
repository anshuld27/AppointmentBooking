using AppointmentBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentBooking.Data
{
    /// <summary>
    /// Database context class for the appointment booking application
    /// </summary>
    public class AppDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions and passes it to the base class
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet properties represent database tables
        public DbSet<SalesManager> SalesManagers { get; set; } = null!;  // Sales managers table
        public DbSet<Slot> Slots { get; set; } = null!;  // Available time slots table

        /// <summary>
        /// Configures the database model and relationships
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration for SalesManager entity
            modelBuilder.Entity<SalesManager>(entity =>
            {
                // Map to 'sales_managers' table in database
                entity.ToTable("sales_managers");

                // Configure array-type columns for PostgreSQL
                entity.Property(e => e.Languages)
                    .HasColumnType("varchar(100)[]");  // Stores array of language proficiencies
                entity.Property(e => e.Products)
                    .HasColumnType("varchar(100)[]");  // Stores array of product specializations
                entity.Property(e => e.CustomerRatings)
                    .HasColumnType("varchar(100)[]");  // Stores array of customer ratings
            });

            // Configuration for Slot entity
            modelBuilder.Entity<Slot>(entity =>
            {
                // Map to 'slots' table in database
                entity.ToTable("slots");

                // Configure timestamp columns with timezone information
                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamptz");  // UTC timestamp for slot start time
                entity.Property(e => e.EndDate)
                    .HasColumnType("timestamptz");  // UTC timestamp for slot end time

                // Configure relationship between Slot and SalesManager
                entity.HasOne(s => s.SalesManager)
                    .WithMany()  // A sales manager can have many slots
                    .HasForeignKey(s => s.SalesManagerId)  // Foreign key in Slots table
                    .HasConstraintName("FK_Slots_SalesManagers");  // Explicit foreign key constraint name
            });

            // Create indexes for query optimization
            // Index on SalesManagerId to improve join performance
            modelBuilder.Entity<Slot>()
                .HasIndex(s => s.SalesManagerId)
                .HasDatabaseName("IX_Slots_SalesManagerId");

            // Index on StartDate for efficient time-based queries
            modelBuilder.Entity<Slot>()
                .HasIndex(s => s.StartDate)
                .HasDatabaseName("IX_Slots_StartDate");

            // Index on EndDate for efficient time-based queries
            modelBuilder.Entity<Slot>()
                .HasIndex(s => s.EndDate)
                .HasDatabaseName("IX_Slots_EndDate");
        }
    }
}