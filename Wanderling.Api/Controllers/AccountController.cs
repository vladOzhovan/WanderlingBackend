using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wanderling.Api.Dtos;
using Wanderling.Infrastructure.Identity;

namespace Wanderling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(ILogger<AccountController> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                // Check email
                var existingByEmail = await _userManager.FindByEmailAsync(dto.Email);
                if (existingByEmail is not null)
                    return BadRequest(new { errors = new[] { "User with this email already exists" } });

                // Check username
                var existingByName = await _userManager.FindByNameAsync(dto.Username);
                if (existingByName is not null)
                    return BadRequest("Username already taken");

                var newUser = new AppUser
                {
                    UserName = dto.Username,
                    Email = dto.Email,
                };

                // Try to create a new user
                var userResult = await _userManager.CreateAsync(newUser, dto.Password);
                if (!userResult.Succeeded)
                {
                    var errorDescriptions = string.Join("; ", userResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to create user {Email}: {Errors}", dto.Email, errorDescriptions);
                    return BadRequest(new { errorDescriptions });
                }
                
                var roleResult = await _userManager.AddToRoleAsync(newUser, "Player");
                if (!roleResult.Succeeded)
                {
                    var errorDescriptions = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to add role Player to user {Email}: {Errors}",dto.Email, errorDescriptions);
                    return BadRequest(new { errorDescriptions });
                }

                return Ok("Player created");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while registering user {Email}", dto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
