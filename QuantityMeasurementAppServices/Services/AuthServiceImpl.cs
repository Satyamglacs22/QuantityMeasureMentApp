using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementAppModels.DTOs;
using QuantityMeasurementAppModels.Entities;
using QuantityMeasurementAppRepositories.Interfaces;
using QuantityMeasurementAppServices.Interfaces;

namespace QuantityMeasurementAppServices.Services
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthServiceImpl> _logger;

        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes    = 15;

        public AuthServiceImpl(
            IAuthRepository authRepository,
            IConfiguration configuration,
            ILogger<AuthServiceImpl> logger)
        {
            _authRepository = authRepository;
            _configuration  = configuration;
            _logger         = logger;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request, string? ipAddress = null)
        {
            try
            {
                bool userExists = await _authRepository.UserExistsAsync(request.Username, request.Email);
                if (userExists)
                    return new AuthResponseDTO { Success = false, Message = "User with this username or email already exists" };

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

                var user = new UserEntity
                {
                    Username            = request.Username,
                    Email               = request.Email,
                    PasswordHash        = passwordHash,
                    FirstName           = request.FirstName,
                    LastName            = request.LastName,
                    CreatedAt           = DateTime.UtcNow,
                    IsActive            = true,
                    Role                = "User",
                    FailedLoginAttempts = 0,
                    LockoutEnd          = null
                };

                UserEntity createdUser = await _authRepository.CreateUserAsync(user);
                var (accessToken, expiresAt) = GenerateJwtToken(createdUser);
                string refreshToken = GenerateRefreshToken();
                await SaveRefreshTokenAsync(createdUser.Id, refreshToken, ipAddress);

                return new AuthResponseDTO
                {
                    Success      = true,
                    Message      = "Registration successful",
                    AccessToken  = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt    = expiresAt,
                    User         = MapToUserInfo(createdUser)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error for: {Username}", request.Username);
                return new AuthResponseDTO { Success = false, Message = "Registration failed" };
            }
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, string? ipAddress = null)
        {
            try
            {
                UserEntity? user = await _authRepository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);

                if (user == null)
                {
                    await Task.Delay(500);
                    return new AuthResponseDTO { Success = false, Message = "Invalid username/email or password" };
                }

                if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                    return new AuthResponseDTO { Success = false, Message = $"Account is locked until {user.LockoutEnd.Value:u}. Please try again later." };

                bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

                if (!isValidPassword)
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts >= MaxFailedAttempts)
                    {
                        user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                        _logger.LogWarning("Account locked: {Username}", user.Username);
                    }
                    await _authRepository.UpdateUserAsync(user);
                    await Task.Delay(500);
                    return new AuthResponseDTO { Success = false, Message = "Invalid username/email or password" };
                }

                if (!user.IsActive)
                    return new AuthResponseDTO { Success = false, Message = "Account is deactivated. Please contact support." };

                user.FailedLoginAttempts = 0;
                user.LockoutEnd         = null;
                user.LastLoginAt        = DateTime.UtcNow;
                await _authRepository.UpdateUserAsync(user);

                var (accessToken, expiresAt) = GenerateJwtToken(user);
                string refreshToken = GenerateRefreshToken();

                await _authRepository.RevokeAllUserTokensAsync(user.Id, ipAddress);
                await SaveRefreshTokenAsync(user.Id, refreshToken, ipAddress);

                return new AuthResponseDTO
                {
                    Success      = true,
                    Message      = "Login successful",
                    AccessToken  = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt    = expiresAt,
                    User         = MapToUserInfo(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for: {UsernameOrEmail}", request.UsernameOrEmail);
                return new AuthResponseDTO { Success = false, Message = "Login failed" };
            }
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
        {
            try
            {
                RefreshTokenEntity? tokenEntity = await _authRepository.GetRefreshTokenAsync(refreshToken);

                if (tokenEntity == null)
                    return new AuthResponseDTO { Success = false, Message = "Invalid refresh token" };

                if (tokenEntity.ExpiresAt < DateTime.UtcNow)
                    return new AuthResponseDTO { Success = false, Message = "Refresh token has expired" };

                if (tokenEntity.RevokedAt != null)
                    return new AuthResponseDTO { Success = false, Message = "Refresh token has been revoked" };

                UserEntity? user = tokenEntity.User;
                if (user == null || !user.IsActive)
                    return new AuthResponseDTO { Success = false, Message = "User not found or deactivated" };

                var (newAccessToken, expiresAt) = GenerateJwtToken(user);
                string newRefreshToken = GenerateRefreshToken();

                tokenEntity.RevokedAt        = DateTime.UtcNow;
                tokenEntity.RevokedByIp      = ipAddress;
                tokenEntity.ReplacedByToken  = newRefreshToken;
                await _authRepository.RevokeRefreshTokenAsync(tokenEntity);
                await SaveRefreshTokenAsync(user.Id, newRefreshToken, ipAddress);

                return new AuthResponseDTO
                {
                    Success      = true,
                    Message      = "Token refreshed successfully",
                    AccessToken  = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt    = expiresAt,
                    User         = MapToUserInfo(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh token error");
                return new AuthResponseDTO { Success = false, Message = "Token refresh failed" };
            }
        }

        public async Task<AuthResponseDTO> LogoutAsync(string? refreshToken = null, long? userId = null, string? ipAddress = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    RefreshTokenEntity? tokenEntity = await _authRepository.GetRefreshTokenAsync(refreshToken);
                    if (tokenEntity != null && tokenEntity.RevokedAt == null)
                    {
                        tokenEntity.RevokedAt    = DateTime.UtcNow;
                        tokenEntity.RevokedByIp  = ipAddress;
                        await _authRepository.RevokeRefreshTokenAsync(tokenEntity);
                    }
                }
                else if (userId.HasValue)
                {
                    await _authRepository.RevokeAllUserTokensAsync(userId.Value, ipAddress);
                }

                return new AuthResponseDTO { Success = true, Message = "Logged out successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                return new AuthResponseDTO { Success = false, Message = "Logout failed" };
            }
        }

        public async Task<UserInfoDTO?> GetUserProfileAsync(long userId)
        {
            UserEntity? user = await _authRepository.GetUserByIdAsync(userId);
            return user == null ? null : MapToUserInfo(user);
        }

        // ==================== Helpers ====================

        private (string token, DateTime expiresAt) GenerateJwtToken(UserEntity user)
        {
            string jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured");

            var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,           user.Username),
                new Claim(JwtRegisteredClaimNames.Email,      user.Email),
                new Claim(ClaimTypes.Role,                    user.Role),
                new Claim(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString())
            };

            DateTime expiresAt = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("Jwt:TokenExpiryInMinutes", 15));

            var token = new JwtSecurityToken(
                issuer:             _configuration["Jwt:Issuer"],
                audience:           _configuration["Jwt:Audience"],
                claims:             claims,
                expires:            expiresAt,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private async Task SaveRefreshTokenAsync(long userId, string token, string? ipAddress)
        {
            var entity = new RefreshTokenEntity
            {
                UserId      = userId,
                Token       = token,
                ExpiresAt   = DateTime.UtcNow.AddDays(
                    _configuration.GetValue<int>("Jwt:RefreshTokenExpiryInDays", 7)),
                CreatedAt   = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
            await _authRepository.CreateRefreshTokenAsync(entity);
        }

        private static UserInfoDTO MapToUserInfo(UserEntity user) => new UserInfoDTO
        {
            Id        = user.Id,
            Username  = user.Username,
            Email     = user.Email,
            FirstName = user.FirstName,
            LastName  = user.LastName,
            Role      = user.Role
        };
    }
}