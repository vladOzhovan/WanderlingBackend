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
                return BadRequest(new { errors = result.Errors});

            return Ok(result.Data);
        }
    }
}
