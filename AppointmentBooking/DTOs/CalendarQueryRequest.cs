using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.DTOs
{
    /// <summary>
    /// Represents a request to query the calendar for available slots.
    /// </summary>
    public class CalendarQueryRequest
    {
        /// <summary>
        /// Gets or sets the date for which to query available slots.
        /// The date must be in the format "yyyy-MM-dd".
        /// </summary>
        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date must be in the format yyyy-MM-dd.")]
        public string Date { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of products to query for.
        /// The list must contain at least one product.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one product must be specified.")]
        public List<string> Products { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the language to query for.
        /// </summary>
        [Required(ErrorMessage = "Language is required.")]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rating to query for.
        /// </summary>
        [Required(ErrorMessage = "Rating is required.")]
        public string Rating { get; set; } = string.Empty;
    }
}
