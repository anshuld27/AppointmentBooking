using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppointmentBooking.DTOs
{
    /// <summary>
    /// Represents the response containing available slot information.
    /// </summary>
    public class AvailableSlotResponse
    {
        private readonly DateTime _startDate;

        /// <summary>
        /// Gets the start date of the available slot.
        /// </summary>
        [Required]
        [JsonPropertyName("start_date")]
        public string StartDate => _startDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        /// <summary>
        /// Gets the count of available slots.
        /// </summary>
        [Range(0, int.MaxValue)]
        [JsonPropertyName("available_count")]
        public int AvailableCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableSlotResponse"/> class.
        /// </summary>
        /// <param name="startDate">The start date of the available slot.</param>
        /// <param name="availableCount">The count of available slots.</param>
        public AvailableSlotResponse(DateTime startDate, int availableCount)
        {
            _startDate = startDate;
            AvailableCount = availableCount;
        }
    }
}
