using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentBooking.Models
{
    /// <summary>
    /// Represents a time slot in the appointment booking system.
    /// </summary>
    [Table("slots")]
    public class Slot
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Slot.
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the Slot.
        /// </summary>
        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of the Slot.
        /// </summary>
        [Required]
        [Column("end_date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Slot is booked.
        /// </summary>
        [Required]
        [Column("booked")]
        public bool Booked { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the Sales Manager associated with the Slot.
        /// </summary>
        [Required]
        [Column("sales_manager_id")]
        public int SalesManagerId { get; set; }

        /// <summary>
        /// Gets or sets the Sales Manager associated with the Slot.
        /// </summary>
        [ForeignKey("SalesManagerId")]
        public SalesManager SalesManager { get; set; } = null!;
    }
}

