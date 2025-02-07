using AppointmentBooking.DTOs;
using AppointmentBooking.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AppointmentBooking.Services
{
    /// <summary>
    /// Service for handling calendar-related operations.
    /// </summary>
    public class CalendarService : ICalendarService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CalendarService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public CalendarService(AppDbContext context, ILogger<CalendarService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets the available slots based on the specified query request.
        /// </summary>
        /// <param name="request">The calendar query request.</param>
        /// <returns>A collection of available slot responses.</returns>
        public async Task<IEnumerable<AvailableSlotResponse>> GetAvailableSlotsAsync(CalendarQueryRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                // Parse the input date string to a DateTime object (UTC)
                var date = DateTime.ParseExact(
                    request.Date,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
                );

                // Calculate the start and end of the day in UTC
                var startOfDayUtc = date.Date.ToUniversalTime();
                var endOfDayUtc = startOfDayUtc.AddDays(1).AddTicks(-1);

                // Fetch all slots (booked and unbooked) for the specified day
                var allSlots = await _context.Slots
                    .Include(s => s.SalesManager)
                    .Where(s => s.StartDate >= startOfDayUtc && s.StartDate <= endOfDayUtc)
                    .ToListAsync();

                // Filter unbooked slots that match the specified criteria
                var eligibleSlots = allSlots
                    .Where(s => !s.Booked)
                    .Where(s => s.SalesManager.Languages.Contains(request.Language))
                    .Where(s => request.Products.All(p => s.SalesManager.Products.Contains(p))) // Ensure all products match
                    .Where(s => s.SalesManager.CustomerRatings.Contains(request.Rating))
                    .ToList();

                // Check for overlapping booked slots
                var availableSlots = eligibleSlots
                    .Where(s => !allSlots.Any(os =>
                        os.SalesManagerId == s.SalesManagerId &&
                        os.Booked &&
                        os.StartDate < s.EndDate &&
                        os.EndDate > s.StartDate
                    ))
                    .ToList();

                // Group available slots by their start time and create response objects
                return availableSlots
                    .GroupBy(s => s.StartDate)
                    .Select(g => new AvailableSlotResponse(g.Key, g.Count()))
                    .OrderBy(s => s.StartDate);
            }
            catch (Exception ex)
            {
                // Log detailed error and rethrow for error handling middleware
                _logger.LogError(ex, "Error retrieving slots for {Date}", request.Date);
                throw;
            }
        }
    }
}
