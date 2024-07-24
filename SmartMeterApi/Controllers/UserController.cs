using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeterApi.Data;
using SmartMeterApi.Models;

namespace SmartMeterApi.Controllers
{
    /// <summary>
    /// this class provides the endpoints for getting users
    /// </summary>
    //only allow Operators to access endpoints
    [Authorize(Roles = "Operator")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly SmartMeterContext _context;

        public UserController(SmartMeterContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Endpoint to get users, only allowed for operators
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public async Task<ActionResult<List<UserDataModel>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Getting user data started.");
                // 1. get only users with role "User"
                List<User> users = await _context.Users.Where(u => u.Role != "Operator").ToListAsync();

                List<UserDataModel> userDataModels = users.Select(u => new UserDataModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname
                }).ToList();

                _logger.LogInformation("Getting user data was successful.");
                // 2. return list of all users
                return Ok(userDataModels);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Getting user data failed: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
