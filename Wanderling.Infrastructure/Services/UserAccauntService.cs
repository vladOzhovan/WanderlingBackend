using Wanderling.Application.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Wanderling.Application.Results;
using Wanderling.Application.Interfaces;
using Wanderling.Infrastructure.Identity;

namespace Wanderling.Infrastructure.Services
{
    public class UserAccauntService : IUserAccauntService
    {
        private const string DEFAULT_ROLE = "Player";

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<UserAccauntService> _logger;
        private readonly ITokenService _tokenService;
        public UserAccauntService(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                ITokenService tokenService,
                                ILogger<UserAccauntService> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<AuthenticationResult> RegisterUserAsync(RegisterDto regDto, CancellationToken ct = default)
        {
            // Check DTO
            if (regDto is null 
                || string.IsNullOrWhiteSpace(regDto.Username)
                || string.IsNullOrWhiteSpace(regDto.Email)
                || string.IsNullOrWhiteSpace(regDto.Password))
                return AuthenticationResult.Fail("Invalid payload");

            var username = regDto.Username.Trim();
            var email = regDto.Email.Trim();
            var password = regDto.Password;

            // Check email
            var existingByEmail = await _userManager.FindByEmailAsync(email);
            if (existingByEmail is not null)
                return AuthenticationResult.Fail("User with this email already exists");

            // Check username
            var existingByName = await _userManager.FindByNameAsync(username);
            if (existingByName is not null)
                return AuthenticationResult.Fail("Username is already taken");

            // Create a new User
            var newUser = new AppUser
            {
                UserName = username,
                Email = email,
            };

            // Try to create a new User
            var userResult = await _userManager.CreateAsync(newUser, password);
            if (!userResult.Succeeded)
            {
                var errorDescriptions = string.Join("; ", userResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to create user {Email}: {Errors}", email, errorDescriptions);
                return AuthenticationResult.Fail($"Failed to create user {email}: {errorDescriptions}");
            }

            // Try to assign a role
            var roleResult = await _userManager.AddToRoleAsync(newUser, "Player");

            return await AuthenticateUserAsync(newUser, ct);
        }

        public async Task<AuthenticationResult> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            // Check DTO
            if (loginDto is null
                || string.IsNullOrWhiteSpace(loginDto.Email)
                || string.IsNullOrWhiteSpace(loginDto.Password))
                return AuthenticationResult.Fail("Invalid payload");

            var email = loginDto.Email.Trim();
            var password = loginDto.Password;

            // Searching for a User
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return AuthenticationResult.Fail("Invalid email or password");
            }

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (!passwordResult.Succeeded)
            {
                _logger.LogWarning("Invalid password for {Email}", email);
                return AuthenticationResult.Fail("Invalid email or password");
            }

            return await AuthenticateUserAsync(user, ct);
        }

        private async Task<AuthenticationResult> AuthenticateUserAsync(AppUser user, CancellationToken ct = default)
        {
            if (user is null)
                return AuthenticationResult.Fail("Invalid user");

            ct.ThrowIfCancellationRequested();

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count == 0)
            {
                var addRoleResult = await _userManager.AddToRoleAsync(user, DEFAULT_ROLE);

                if (!addRoleResult.Succeeded)
                {
                    var errors = addRoleResult.Errors.Select(e => e.Description).ToArray();
                    _logger.LogWarning(
                        "Failed to auto-add role {Role} to user {UserId}. {Errors}:",
                        DEFAULT_ROLE,
                        user.Id,
                        string.Join("; ", errors)
                    );
                    return AuthenticationResult.Fail("Invalid user role");
                }

                roles = await _userManager.GetRolesAsync(user);
            }

            var tokenDto = BuildTokenDto(user, roles);
            var token = _tokenService.GenerateToken(tokenDto);

            var authDto = BuildAuthenticationDto(user, roles, token);
            return AuthenticationResult.Ok(authDto);
        }

        private AuthenticationDto BuildAuthenticationDto(AppUser user, IEnumerable<string> roles, string token)
        {
            return new AuthenticationDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToArray(),
                Token = token
            };
        }

        private TokenDto BuildTokenDto(AppUser user, IEnumerable<string> roles)
        {
            return new TokenDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToArray()
            };
        }
    }
}
