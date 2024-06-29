using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeterApi.Data;
using SmartMeterApi.Models;


namespace SmartMeterApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ConsumptionController : ControllerBase
	{
		private readonly SmartMeterContext _context;

		public ConsumptionController(SmartMeterContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> AddConsumptionData(ConsumptionData data)
		{
			_context.ConsumptionDatas.Add(data);
			await _context.SaveChangesAsync();
			return Ok();
		}

		[HttpGet("{userId}")]
		public async Task<ActionResult<List<ConsumptionData>>> GetConsumptionData(string userId)
		{
			return await _context.ConsumptionDatas.Where(d => d.UserId == userId).ToListAsync();
		}
	}
}
