using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using QuantityMeasurementAppModels.DTOs;
using QuantityMeasurementAppModels.Entities;
using QuantityMeasurementAppRepositories.Interfaces;
using QuantityMeasurementAppServices.Interfaces;
using QuantityMeasurementWebApi.Controllers;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IAuthService>               _mockAuthService = null!;
        private Mock<IAuthRepository>            _mockAuthRepo    = null!;
        private Mock<ILogger<AuthController>>    _mockLogger      = null!;
        private IConfiguration                   _configuration   = null!;
        private AuthController                   _controller      = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockAuthRepo    = new Mock<IAuthRepository>();
            _mockLogger      = new Mock<ILogger<AuthController>>();

            var configData = new Dictionary<string, string?>
            {
                ["Jwt:Key"]                             = "TestSuperSecretKeyForUnitTestsThatIsAtLeast32Chars!",
                ["Jwt:Issuer"]                          = "TestIssuer",
                ["Jwt:Audience"]                        = "TestAudience",
                ["Jwt:TokenExpiryInMinutes"]            = "15",
                ["Jwt:RefreshTokenExpiryInDays"]        = "7",
                ["Authentication:Google:ClientId"]      = "test-google-client-id",
                ["Authentication:Google:ClientSecret"]  = "test-google-client-secret",
                ["Authentication:Google:RedirectUri"]   = "http://localhost:5000/api/v1/auth/google/callback",
                ["Frontend:Url"]                        = "http://localhost:3000"
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            _controller = new AuthController(
                _mockAuthService.Object,
                _mockAuthRepo.Object,
                _configuration,
                _mockLogger.Object);

            // Default HTTP context with a remote IP
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        // ==================== Register ====================

        [TestMethod]
        public async Task Register_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Username        = "newuser",
                Email           = "new@example.com",
                Password        = "Test123!",
                ConfirmPassword = "Test123!",
                FirstName       = "New",
                LastName        = "User"
            };

            var response = new AuthResponseDTO
            {
                Success      = true,
                Message      = "Registration successful",
                AccessToken  = "access.token.value",
                RefreshToken = "refresh-token-value",
                ExpiresAt    = DateTime.UtcNow.AddHours(1)
            };

            _mockAuthService
                .Setup(s => s.RegisterAsync(request, "127.0.0.1"))
                .ReturnsAsync(response);

            // Act
            IActionResult result = await _controller.Register(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Register_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequestDTO();
            _controller.ModelState.AddModelError("Username", "Required");

            // Act
            IActionResult result = await _controller.Register(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_DuplicateUser_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Username = "existing",
                Email    = "existing@example.com",
                Password = "Test123!",
                FirstName = "Ex",
                LastName  = "Isting"
            };

            var response = new AuthResponseDTO
            {
                Success = false,
                Message = "User with this username or email already exists"
            };

            _mockAuthService
                .Setup(s => s.RegisterAsync(request, "127.0.0.1"))
                .ReturnsAsync(response);

            // Act
            IActionResult result = await _controller.Register(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        // ==================== Login ====================

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var request = new LoginRequestDTO
            {
                UsernameOrEmail = "testuser",
                Password        = "Test123!"
            };

            var response = new AuthResponseDTO
            {
                Success      = true,
                Message      = "Login successful",
                AccessToken  = "access.token.value",
                RefreshToken = "refresh-token-value",
                ExpiresAt    = DateTime.UtcNow.AddHours(1)
            };

            _mockAuthService
                .Setup(s => s.LoginAsync(request, "127.0.0.1"))
                .ReturnsAsync(response);

            // Act
            IActionResult result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequestDTO
            {
                UsernameOrEmail = "testuser",
                Password        = "WrongPassword!"
            };

            var response = new AuthResponseDTO
            {
                Success = false,
                Message = "Invalid username/email or password"
            };

            _mockAuthService
                .Setup(s => s.LoginAsync(request, "127.0.0.1"))
                .ReturnsAsync(response);

            // Act
            IActionResult result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        // ==================== RefreshToken ====================

        [TestMethod]
        public async Task RefreshToken_WithValidCookie_ReturnsOk()
        {
            // Arrange
            string tokenValue = "valid-refresh-token";

            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c["refreshToken"]).Returns(tokenValue);

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);

            var mockResponseCookies = new Mock<IResponseCookies>();
            var mockResponse        = new Mock<HttpResponse>();
            mockResponse.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Request).Returns(mockRequest.Object);
            mockContext.Setup(c => c.Response).Returns(mockResponse.Object);
            mockContext.Setup(c => c.Connection.RemoteIpAddress)
                .Returns(System.Net.IPAddress.Parse("127.0.0.1"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockContext.Object
            };

            var response = new AuthResponseDTO
            {
                Success      = true,
                Message      = "Token refreshed successfully",
                AccessToken  = "new-access-token",
                RefreshToken = "new-refresh-token",
                ExpiresAt    = DateTime.UtcNow.AddHours(1)
            };

            _mockAuthService
                .Setup(s => s.RefreshTokenAsync(tokenValue, "127.0.0.1"))
                .ReturnsAsync(response);

            // Act
            IActionResult result = await _controller.RefreshToken();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task RefreshToken_NoCookieOrBody_ReturnsBadRequest()
        {
            // Arrange — empty cookie
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c["refreshToken"]).Returns(string.Empty);

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);
            // Body returns null/empty
            mockRequest.Setup(r => r.Body).Returns(new System.IO.MemoryStream());

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Request).Returns(mockRequest.Object);
            mockContext.Setup(c => c.Connection.RemoteIpAddress)
                .Returns(System.Net.IPAddress.Parse("127.0.0.1"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockContext.Object
            };

            // Act
            IActionResult result = await _controller.RefreshToken();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        // ==================== Logout ====================

        [TestMethod]
        public async Task Logout_AuthenticatedUser_ReturnsOk()
        {
            // Arrange — set authenticated user claims
            var claims    = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity  = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;

            var response = new AuthResponseDTO { Success = true, Message = "Logged out successfully" };

            _mockAuthService
                .Setup(s => s.LogoutAsync(null, 1L, "127.0.0.1"))
                .ReturnsAsync(response);

            // Act
            IActionResult result = await _controller.Logout(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Logout_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // No claims set — user identity is empty
            IActionResult result = await _controller.Logout(null);

            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        // ==================== GetProfile ====================

        [TestMethod]
        public async Task GetProfile_AuthenticatedUser_ReturnsOk()
        {
            // Arrange
            var claims    = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity  = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;

            var profile = new UserInfoDTO
            {
                Id        = 1,
                Username  = "testuser",
                Email     = "test@example.com",
                FirstName = "Test",
                LastName  = "User",
                Role      = "User"
            };

            _mockAuthService.Setup(s => s.GetUserProfileAsync(1)).ReturnsAsync(profile);

            // Act
            IActionResult result = await _controller.GetProfile();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var ok = (OkObjectResult)result;
            Assert.AreEqual(profile, ok.Value);
        }

        [TestMethod]
        public async Task GetProfile_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // No claims on HttpContext
            IActionResult result = await _controller.GetProfile();

            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public async Task GetProfile_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var claims    = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "999") };
            var identity  = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;

            _mockAuthService.Setup(s => s.GetUserProfileAsync(999))
                .ReturnsAsync((UserInfoDTO?)null);

            // Act
            IActionResult result = await _controller.GetProfile();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        // ==================== GetAuthStatus ====================

        [TestMethod]
        public void GetAuthStatus_AuthenticatedUser_ReturnsIsAuthenticatedTrue()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
            };
            var identity  = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;

            // Act
            IActionResult result = _controller.GetAuthStatus();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var ok = (OkObjectResult)result;
            // Verify the anonymous object contains IsAuthenticated = true
            var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
            Assert.IsTrue(json.Contains("true"));
        }

        [TestMethod]
        public void GetAuthStatus_UnauthenticatedUser_ReturnsIsAuthenticatedFalse()
        {
            // No claims — default HttpContext
            IActionResult result = _controller.GetAuthStatus();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var ok   = (OkObjectResult)result;
            var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
            Assert.IsTrue(json.Contains("false"));
        }

        // ==================== GoogleLogin ====================

        [TestMethod]
        public void GoogleLogin_ReturnsRedirectToGoogle()
        {
            // Act
            IActionResult result = _controller.GoogleLogin();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirect = (RedirectResult)result;
            Assert.IsTrue(redirect.Url.Contains("accounts.google.com"));
            Assert.IsTrue(redirect.Url.Contains("test-google-client-id"));
        }
    }
}
