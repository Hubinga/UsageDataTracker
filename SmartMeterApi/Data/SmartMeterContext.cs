using Microsoft.EntityFrameworkCore;
using SmartMeterApi.Models;

namespace SmartMeterApi.Data
{
	public class SmartMeterContext : DbContext
	{
		public SmartMeterContext(DbContextOptions<SmartMeterContext> options) : base(options) { }

		/// <summary>
		/// DbSet representing the ConsumptionData table in the database.
		/// </summary>
		public DbSet<ConsumptionData> ConsumptionDatas { get; set; }
		/// <summary>
		/// DbSet representing the Users table in the database.
		/// </summary>
		public DbSet<User> Users { get; set; }
	}
}