using AppointmentBooking.DTOs;
using AppointmentBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentBooking.Controllers
{
    /// <summary>
    /// Controller for handling calendar-related operations
    /// </summary>
    [ApiController]
    [Route("calendar")]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        /// <summary>
        /// Initializes a new instance of the CalendarController
        /// </summary>
        /// <param name="calendarService">Calendar service dependency</param>
        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        /// <summary>
        /// Retrieves available time slots based on the provided query parameters
        /// </summary>
        /// <param name="request">Query parameters including date range and required duration</param>
        /// <returns>
        /// Returns available time slots if successful, 
        /// error messages for invalid requests or server errors
        /// </returns>
        /// <remarks>
        /// Sample request:
        /// POST /calendar/query
        /// {
        ///"date": "2024-05-03",
        ///"products": ["SolarPanels", "Heatpumps"],
        ///"language": "German",
        ///"rating": "Gold"
        ///}
        /// </remarks>
        [HttpPost("query")]
        public async Task<IActionResult> QueryAvailableSlots([FromBody] CalendarQueryRequest request)
        {
            // Validate the request model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get available slots from calendar service
                var availableSlots = await _calendarService.GetAvailableSlotsAsync(request);
                return Ok(availableSlots);
            }
            catch (FormatException)
            {
                // Handle invalid date format exceptions
                return BadRequest("Invalid date format.");
            }
            catch (Exception ex)
            {
                // Handle unexpected errors and return generic error message
                // Note: In production, log the exception details and avoid exposing sensitive information
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}