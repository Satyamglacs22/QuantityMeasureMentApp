using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuantityMeasurementAppModels.Entities;
using QuantityMeasurementAppRepositories.Context;

namespace QuantityMeasurementWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext            _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger  = logger;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new { u.Id, u.Username, u.Email, u.FirstName, u.LastName, u.CreatedAt, u.LastLoginAt, u.IsActive, u.Role })
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                return StatusCode(500, new { Message = "An error occurred while retrieving users" });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                int total  = await _context.Users.CountAsync();
                int active = await _context.Users.CountAsync(u => u.IsActive);
                int admins = await _context.Users.CountAsync(u => u.Role == "Admin");
                return Ok(new { TotalUsers = total, ActiveUsers = active, InactiveUsers = total - active, AdminUsers = admins, RegularUsers = total - admins });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching statistics");
                return StatusCode(500, new { Message = "An error occurred" });
            }
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(long id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                UserEntity? user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound(new { Message = "User not found" });
                if (string.IsNullOrEmpty(request.Role) || (request.Role != "User" && request.Role != "Admin"))
                    return BadRequest(new { Message = "Invalid role. Must be 'User' or 'Admin'" });
                user.Role = request.Role;
                await _context.SaveChangesAsync();
                return Ok(new { Message = "User role updated successfully", user.Role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role for user: {Id}", id);
                return StatusCode(500, new { Message = "An error occurred" });
            }
        }

        [HttpPut("users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(long id)
        {
            try
            {
                UserEntity? user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound(new { Message = "User not found" });
                user.IsActive = false;
                await _context.SaveChangesAsync();
                return Ok(new { Message = $"User {user.Username} has been deactivated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user: {Id}", id);
                return StatusCode(500, new { Message = "An error occurred" });
            }
        }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}