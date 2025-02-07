using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentBooking.Models
{
    /// <summary>
    /// Represents a Sales Manager in the appointment booking system.
    /// </summary>
    [Table("sales_managers")]
    public class SalesManager
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Sales Manager.
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the Sales Manager.
        /// </summary>
        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of languages the Sales Manager is proficient in.
        /// </summary>
        [Column("languages")]
        public List<string> Languages { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of products the Sales Manager is responsible for.
        /// </summary>
        [Column("products")]
        public List<string> Products { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of customer ratings for the Sales Manager.
        /// </summary>
        [Column("customer_ratings")]
        public List<string> CustomerRatings { get; set; } = new List<string>();
    }
}
