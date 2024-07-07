using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
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

        /// year.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// month
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// The actual consumption value recorded.
        /// </summary>
        public int ConsumptionValue { get; set; }
	}

}