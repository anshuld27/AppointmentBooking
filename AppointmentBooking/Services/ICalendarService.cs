using AppointmentBooking.DTOs;

namespace AppointmentBooking.Services
{
    /// <summary>
    /// Provides calendar availability operations
    /// </summary>
    public interface ICalendarService
    {
        /// <summary>
        /// Finds available time slots matching query parameters
        /// </summary>
        /// <param name="request">Contains filters like date range and service type</param>
        Task<IEnumerable<AvailableSlotResponse>> GetAvailableSlotsAsync(CalendarQueryRequest request);
    }
}