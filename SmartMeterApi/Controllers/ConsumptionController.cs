using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeterApi.Data;
using SmartMeterApi.Models;
using System.Security.Claims;

namespace SmartMeterApi.Controllers
{
    /// <summary>
    /// this class provides the endpoints for usage data
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumptionController : ControllerBase
	{
		private readonly SmartMeterContext _context;

		public ConsumptionController(SmartMeterContext context)
		{
			_context = context;
		}

        /// <summary>
        /// Endpoint to add usage data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "User, Operator")]
        [HttpPost("add")]
        public async Task<IActionResult> AddOrUpdateConsumptionData([FromBody] UsageDataModel data)
        {
            // 1. extract UserId and Role from JWT Token
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            try
            {
                // 2. Check role: only Operator is allowed to add or update data for each user
                if (userRole != "Operator" && currentUserId != data.UserId)
                {
                    return Forbid("You are not allowed to add or update data for another user.");
                }

                // 3. Check if the data for the given user, year, and month already exists
                var existingData = await _context.ConsumptionDatas.FirstOrDefaultAsync(d => d.UserId == data.UserId && d.Year == data.Year && d.Month == data.Month);

                if (existingData != null)
                {
                    // Update the existing data
                    existingData.ConsumptionValue = data.Value;
                    _context.ConsumptionDatas.Update(existingData);
                }
                else
                {
                    // Add new data
                    var consumptionData = new ConsumptionData
                    {
                        UserId = data.UserId,
                        Year = data.Year,
                        Month = data.Month,
                        ConsumptionValue = data.Value
                    };

                    _context.ConsumptionDatas.Add(consumptionData);
                }

                // 4. Save changes
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Endpoint to get usage data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of usage data</returns>
        [Authorize(Roles = "User, Operator")]
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<ConsumptionData>>> GetConsumptionData(string userId)
        {
            // 1. extract UserId from JWT Token
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Check role: only Operator is allowed to get data for each user
            if (currentUserId != userId && !User.IsInRole("Operator"))
            {
                return Forbid("You are not allowed to access data for another user.");
            }

            try
            {
                // 3. get data for user with userId
                List<ConsumptionData> data = await _context.ConsumptionDatas.Where(d => d.UserId == userId).ToListAsync();

				List<UsageDataModel> usageDataModels = data.Select(d => new UsageDataModel
				{
					UserId = d.UserId,
					Year = d.Year,
					Month = d.Month,
					Value = d.ConsumptionValue
				}).ToList();

                // 4. return List of usage data
				return Ok(usageDataModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
