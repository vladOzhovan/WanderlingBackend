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

        public async Task<Result<RegistrationDto>> RegisterUserAsync(RegisterDto regDto, CancellationToken ct = default)
        {
            // Check DTO
            if (regDto is null 
                || string.IsNullOrWhiteSpace(regDto.Username)
                || string.IsNullOrWhiteSpace(regDto.Email)
                || string.IsNullOrWhiteSpace(regDto.Password))
                return Result<RegistrationDto>.Fail(new Error(ErrorCodes.InvalidPayload, "Invalid payload"));

            var username = regDto.Username.Trim();
            var email = regDto.Email.Trim();
            var password = regDto.Password;

            // Check email
            var existingByEmail = await _userManager.FindByEmailAsync(email);
            if (existingByEmail is not null)
                return Result<RegistrationDto>.Fail(new Error(ErrorCodes.DuplicateEmail, "User with this email already exists"));

            // Check username
            var existingByName = await _userManager.FindByNameAsync(username);
            if (existingByName is not null)
                return Result<RegistrationDto>.Fail(new Error(ErrorCodes.DuplicateUsername, "Username is already taken"));

            // Create a new User
            var newUser = new AppUser
            {
                UserName = username,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            // Try to create a new User
            var userResult = await _userManager.CreateAsync(newUser, password);
            if (!userResult.Succeeded)
            {
                var errorDescriptions = string.Join("; ", userResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to create user. Email={Email}: Errors={Errors}", email, errorDescriptions);
                return Result<RegistrationDto>.Fail(new Error(ErrorCodes.IdentityCreateFailed, errorDescriptions));
            }

            // Try to assign a role
            var roleResult = await _userManager.AddToRoleAsync(newUser, DEFAULT_ROLE);
            if (!roleResult.Succeeded)
            {
                var errorDescriptions = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to add role {Role} to user {Email}: {Errors}", DEFAULT_ROLE, email, errorDescriptions);
                await _userManager.DeleteAsync(newUser);
                return Result<RegistrationDto>.Fail(new Error(ErrorCodes.RoleAssignFailed, errorDescriptions));
            }

            var roles = await _userManager.GetRolesAsync(newUser);

            var dto = new RegistrationDto
            {
                UserId = newUser.Id,
                Username = newUser.UserName ?? string.Empty,
                Email = newUser.Email ?? string.Empty,
                Roles = roles.ToArray()
            };

            return Result<RegistrationDto>.Ok(dto);
        }

        public async Task<Result<AuthenticationDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            // Check DTO
            if (loginDto is null
                || string.IsNullOrWhiteSpace(loginDto.Email)
                || string.IsNullOrWhiteSpace(loginDto.Password))
                return Result<AuthenticationDto>.Fail(new Error(ErrorCodes.InvalidPayload, "Invalid payload"));

            var email = loginDto.Email.Trim();
            var password = loginDto.Password;

            // Searching for a User
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Result<AuthenticationDto>.Fail(new Error(ErrorCodes.InvalidCredentials, "Invalid email or password"));

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (!passwordResult.Succeeded)
                return Result<AuthenticationDto>.Fail(new Error(ErrorCodes.InvalidCredentials, "Invalid email or password"));

            return await AuthenticateUserAsync(user, ct);
        }

        private async Task<Result<AuthenticationDto>> AuthenticateUserAsync(AppUser user, CancellationToken ct = default)
        {
            if (user is null)
                return Result<AuthenticationDto>.Fail(new Error(ErrorCodes.InvalidCredentials, "Invalid user"));

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
                    return Result<AuthenticationDto>.Fail(new Error(ErrorCodes.InvalidUserRole, "Invalid user role"));
                }

                roles = await _userManager.GetRolesAsync(user);
            }

            var tokenDto = BuildTokenDto(user, roles);
            var token = _tokenService.GenerateToken(tokenDto);

            var authDto = BuildAuthenticationDto(user, roles, token);
            return Result<AuthenticationDto>.Ok(authDto);
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
