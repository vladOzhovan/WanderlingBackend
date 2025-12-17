using Microsoft.AspNetCore.Mvc;
using Wanderling.Application.Dtos;
using Wanderling.Application.Interfaces;

namespace Wanderling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserAccauntService _userAccountService;
        public AccountController(ILogger<AccountController> logger, IUserAccauntService userAccountService)
        {
            _logger = logger;
            _userAccountService = userAccountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct = default)
        {
            var result = await _userAccountService.RegisterUserAsync(dto, ct);

            if (!result.Succeeded)
                return BadRequest(result);

            _logger.LogInformation("User registred. UserId={UserId}. TraceId={TraceId}", result?.Data?.UserId, HttpContext.TraceIdentifier);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct = default)
        {
            var result = await _userAccountService.LoginAsync(dto, ct);

            if (!result.Succeeded)
                return BadRequest(result);

            _logger.LogInformation("User logged in. UserId={UserId}. TraceId={TraceId}", result?.Data?.UserId, HttpContext.TraceIdentifier);

            return Ok(result);
        }
    }
}
