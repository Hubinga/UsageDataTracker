using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeterApi.Data;
using SmartMeterApi.Models;
using System.Security.Claims;

namespace SmartMeterApi.Controllers
{
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

        [HttpPost("add")]
        public async Task<IActionResult> AddConsumptionData([FromBody] UsageDataModel data)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Operator is allowed to add data for each user
            if (userRole != "Operator" && currentUserId != data.UserId)
            {
                return Forbid("You are not allowed to add data for another user.");
            }

            var consumptionData = new ConsumptionData
            {
                UserId = data.UserId,
                Year = data.Year,
                Month = data.Month,
                ConsumptionValue = data.Value
            };

            _context.ConsumptionDatas.Add(consumptionData);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(consumptionData);
            }
            catch (Exception ex)
            {
                // Log exception details (ex)
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "User,Operator")]
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<ConsumptionData>>> GetConsumptionData(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != userId && !User.IsInRole("Operator"))
            {
                return Forbid("You are not allowed to access data for another user.");
            }

            try
            {
                //get data for user with userId
                List<ConsumptionData> data = await _context.ConsumptionDatas.Where(d => d.UserId == userId).ToListAsync();

				List<UsageDataModel> usageDataModels = data.Select(d => new UsageDataModel
				{
					UserId = d.UserId,
					Year = d.Year,
					Month = d.Month,
					Value = d.ConsumptionValue
				}).ToList();

				return Ok(usageDataModels);
            }
            catch (Exception ex)
            {
                // Log exception details (ex)
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
