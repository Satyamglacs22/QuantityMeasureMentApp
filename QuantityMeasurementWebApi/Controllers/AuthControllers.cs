using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementAppModels.DTOs;
using QuantityMeasurementAppModels.Entities;
using QuantityMeasurementAppRepositories.Interfaces;
using QuantityMeasurementAppServices.Interfaces;

namespace QuantityMeasurementWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService            _authService;
        private readonly IAuthRepository         _authRepository;
        private readonly IConfiguration          _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService            authService,
            IAuthRepository         authRepository,
            IConfiguration          configuration,
            ILogger<AuthController> logger)
        {
            _authService    = authService;
            _authRepository = authRepository;
            _configuration  = configuration;
            _logger         = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            AuthResponseDTO result = await _authService.RegisterAsync(request, ipAddress);

            if (!result.Success) return BadRequest(result);

            if (result.RefreshToken != null && result.ExpiresAt.HasValue)
                SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt.Value.AddDays(7));

            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            AuthResponseDTO result = await _authService.LoginAsync(request, ipAddress);

            if (!result.Success) return Unauthorized(result);

            if (result.RefreshToken != null && result.ExpiresAt.HasValue)
                SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt.Value.AddDays(7));

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            string? refreshToken = Request.Cookies["refreshToken"] ?? await GetRefreshTokenFromBody();

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new AuthResponseDTO { Success = false, Message = "Refresh token is required" });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            AuthResponseDTO result = await _authService.RefreshTokenAsync(refreshToken, ipAddress);

            if (!result.Success) return Unauthorized(result);

            if (result.RefreshToken != null && result.ExpiresAt.HasValue)
                SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt.Value.AddDays(7));

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO? request = null)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new AuthResponseDTO { Success = false, Message = "User not authenticated" });

            long userId      = long.Parse(userIdClaim);
            string? token    = request?.RefreshToken ?? Request.Cookies["refreshToken"];
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            AuthResponseDTO result = await _authService.LogoutAsync(token, userId, ipAddress);
            Response.Cookies.Delete("refreshToken");
            return Ok(result);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = "User not authenticated" });

            UserInfoDTO? profile = await _authService.GetUserProfileAsync(long.Parse(userIdClaim));
            if (profile == null) return NotFound(new { Message = "User not found" });

            return Ok(profile);
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public IActionResult GetAuthStatus()
        {
            bool isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            return Ok(new
            {
                IsAuthenticated = isAuthenticated,
                Username        = User.Identity?.Name,
                UserId          = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Message         = isAuthenticated ? "User is logged in" : "User is not logged in"
            });
        }

        [HttpGet("google/login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string redirectUri = _configuration["Authentication:Google:RedirectUri"]
                ?? "http://localhost:5000/api/v1/auth/google/callback";
            string? clientId = _configuration["Authentication:Google:ClientId"];

            string url = "https://accounts.google.com/o/oauth2/v2/auth?"
                + $"client_id={clientId}&"
                + $"redirect_uri={Uri.EscapeDataString(redirectUri)}&"
                + "response_type=code&"
                + "scope=openid%20email%20profile&"
                + "access_type=offline&"
                + "prompt=consent";

            return Redirect(url);
        }

        [HttpGet("google/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback([FromQuery] string? code, [FromQuery] string? error)
        {
            string frontendUrl = _configuration["Frontend:Url"] ?? "http://localhost:3000";

            try
            {
                if (!string.IsNullOrEmpty(error))
                    return Redirect($"{frontendUrl}/login?error={Uri.EscapeDataString(error)}");

                if (string.IsNullOrEmpty(code))
                    return Redirect($"{frontendUrl}/login?error=No+authorization+code");

                GoogleTokenResponse? tokenResponse = await ExchangeCodeForTokensAsync(code);
                if (tokenResponse == null)
                    return Redirect($"{frontendUrl}/login?error=Failed+to+exchange+code");

                GoogleUserInfo? googleUser = await GetGoogleUserInfoAsync(tokenResponse.access_token);
                if (googleUser == null || string.IsNullOrEmpty(googleUser.email))
                    return Redirect($"{frontendUrl}/login?error=Failed+to+get+user+info");

                UserEntity? user = await _authRepository.GetUserByEmailAsync(googleUser.email);

                if (user == null)
                {
                    user = new UserEntity
                    {
                        Username     = googleUser.email.Split('@')[0],
                        Email        = googleUser.email,
                        PasswordHash = $"GOOGLE_AUTH_{googleUser.id}",
                        FirstName    = googleUser.given_name  ?? string.Empty,
                        LastName     = googleUser.family_name ?? string.Empty,
                        CreatedAt    = DateTime.UtcNow,
                        IsActive     = true,
                        Role         = "User"
                    };
                    user = await _authRepository.CreateUserAsync(user);
                }

                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                var (accessToken, _) = GenerateJwtToken(user);
                string refreshToken  = GenerateRefreshToken();

                await _authRepository.RevokeAllUserTokensAsync(user.Id, ipAddress);
                await SaveRefreshTokenAsync(user.Id, refreshToken, ipAddress);

                string userJson = Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(new
                {
                    user.Id, user.Username, user.Email,
                    user.FirstName, user.LastName, user.Role
                }));

                return Redirect($"{frontendUrl}/auth/callback?accessToken={accessToken}&refreshToken={refreshToken}&user={userJson}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google callback error");
                return Redirect($"{frontendUrl}/login?error=Internal+server+error");
            }
        }

        // ==================== Helpers ====================

        private async Task<GoogleTokenResponse?> ExchangeCodeForTokensAsync(string code)
        {
            string? clientId     = _configuration["Authentication:Google:ClientId"];
            string? clientSecret = _configuration["Authentication:Google:ClientSecret"];
            string redirectUri   = _configuration["Authentication:Google:RedirectUri"]
                ?? "http://localhost:5000/api/v1/auth/google/callback";

            using var http = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code",          code),
                new KeyValuePair<string, string>("client_id",     clientId!),
                new KeyValuePair<string, string>("client_secret", clientSecret!),
                new KeyValuePair<string, string>("redirect_uri",  redirectUri),
                new KeyValuePair<string, string>("grant_type",    "authorization_code")
            });

            var response = await http.PostAsync("https://oauth2.googleapis.com/token", content);
            string body  = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) { _logger.LogError("Token exchange failed: {Body}", body); return null; }
            return System.Text.Json.JsonSerializer.Deserialize<GoogleTokenResponse>(body);
        }

        private async Task<GoogleUserInfo?> GetGoogleUserInfoAsync(string accessToken)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await http.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
            string body  = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) { _logger.LogError("Failed to get Google user info: {Body}", body); return null; }
            return System.Text.Json.JsonSerializer.Deserialize<GoogleUserInfo>(body);
        }

        private (string token, DateTime expiresAt) GenerateJwtToken(UserEntity user)
        {
            string jwtKey   = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
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
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:TokenExpiryInMinutes", 15));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], audience: _configuration["Jwt:Audience"],
                claims: claims, expires: expiresAt, signingCredentials: credentials);
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
            await _authRepository.CreateRefreshTokenAsync(new RefreshTokenEntity
            {
                UserId      = userId,
                Token       = token,
                ExpiresAt   = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpiryInDays", 7)),
                CreatedAt   = DateTime.UtcNow,
                CreatedByIp = ipAddress
            });
        }

        private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true, Secure = false, SameSite = SameSiteMode.Lax,
                Expires = expires, Path = "/", IsEssential = true
            });
        }

        private async Task<string?> GetRefreshTokenFromBody()
        {
            try
            {
                Request.EnableBuffering();
                using var reader = new StreamReader(Request.Body, leaveOpen: true);
                string body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
                if (!string.IsNullOrEmpty(body))
                {
                    var doc = System.Text.Json.JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("refreshToken", out var elem))
                        return elem.GetString();
                }
            }
            catch { }
            return null;
        }

        public class GoogleTokenResponse
        {
            public string access_token  { get; set; } = string.Empty;
            public string refresh_token { get; set; } = string.Empty;
            public string id_token      { get; set; } = string.Empty;
            public int    expires_in    { get; set; }
        }

        public class GoogleUserInfo
        {
            public string id          { get; set; } = string.Empty;
            public string email       { get; set; } = string.Empty;
            public string given_name  { get; set; } = string.Empty;
            public string family_name { get; set; } = string.Empty;
            public string name        { get; set; } = string.Empty;
            public string picture     { get; set; } = string.Empty;
        }
    }
}