using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeterApi.Data;
using SmartMeterApi.Models;

namespace SmartMeterApi.Controllers
{
    [Authorize(Roles = "Operator")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly SmartMeterContext _context;

        public UserController(SmartMeterContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDataModel>>> GetUsers()
        {
            try
            {
                // get only users with role "User"
                List<User> users = await _context.Users.Where(u => u.Role != "Operator").ToListAsync();

                List<UserDataModel> userDataModels = users.Select(u => new UserDataModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname
                }).ToList();

                return Ok(userDataModels);
            }
            catch (Exception ex)
            {
                // Log exception details (ex)
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
