using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
    /*Sicherheitsprinzipien: 
     - Eingabevalidierung: Alle Benutzereingaben werden überprüft, um sicherzustellen, dass sie den erwarteten Formaten entsprechen und keine schädlichen Daten enthalten.
     - Verhinderung von Injection-Angriffen: Durch die Verwendung von regulären Ausdrücken wird sichergestellt, dass nur erlaubte Zeichen akzeptiert werden.*/

    /// <summary>
    /// Represents consumption data for a user.
    /// </summary>
    public class ConsumptionData
	{
        /// <summary>
        /// Unique identifier for the consumption data entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// User ID associated with the consumption data.
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Year of the consumption data.
        /// </summary>
        [Required(ErrorMessage = "Year is required.")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }

        /// <summary>
        /// Month of the consumption data.
        /// </summary>
        [Required(ErrorMessage = "Month is required.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Month must contain only letters.")]
        [StringLength(12, ErrorMessage = "Month must be a maximum of 12 characters long.")]
        public string Month { get; set; }

        /// <summary>
        /// The actual consumption value recorded.
        /// </summary>
        [Required(ErrorMessage = "Value is required")]
        [Range(0, 10000, ErrorMessage = "Consumption value must be between 0 and 10000.")]
        public int ConsumptionValue { get; set; }
    }

}