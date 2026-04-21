using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using QuantityMeasurementAppModels.DTOs;
using QuantityMeasurementAppModels.Entities;
using QuantityMeasurementAppRepositories.Interfaces;
using QuantityMeasurementAppServices.Services;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IAuthRepository>       _mockAuthRepo  = null!;
        private Mock<ILogger<AuthServiceImpl>> _mockLogger = null!;
        private IConfiguration              _configuration = null!;
        private AuthServiceImpl             _authService   = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockAuthRepo = new Mock<IAuthRepository>();
            _mockLogger   = new Mock<ILogger<AuthServiceImpl>>();

            var configData = new Dictionary<string, string?>
            {
                ["Jwt:Key"]                    = "TestSuperSecretKeyForUnitTestsThatIsAtLeast32Chars!",
                ["Jwt:Issuer"]                 = "TestIssuer",
                ["Jwt:Audience"]               = "TestAudience",
                ["Jwt:TokenExpiryInMinutes"]   = "60",
                ["Jwt:RefreshTokenExpiryInDays"] = "7"
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            _authService = new AuthServiceImpl(
                _mockAuthRepo.Object,
                _configuration,
                _mockLogger.Object);
        }

        // ==================== RegisterAsync ====================

        [TestMethod]
        public async Task RegisterAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Username        = "testuser",
                Email           = "test@example.com",
                Password        = "Test123!",
                ConfirmPassword = "Test123!",
                FirstName       = "Test",
                LastName        = "User"
            };

            _mockAuthRepo.Setup(r => r.UserExistsAsync(request.Username, request.Email))
                .ReturnsAsync(false);

            _mockAuthRepo.Setup(r => r.CreateUserAsync(It.IsAny<UserEntity>()))
                .ReturnsAsync((UserEntity u) =>
                {
                    u.Id = 1;
                    return u;
                });

            _mockAuthRepo.Setup(r => r.CreateRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .ReturnsAsync((RefreshTokenEntity rt) => rt);

            // Act
            AuthResponseDTO result = await _authService.RegisterAsync(request);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Registration successful", result.Message);
            Assert.IsNotNull(result.AccessToken);
            Assert.IsNotNull(result.RefreshToken);
            Assert.IsNotNull(result.User);
            Assert.AreEqual(request.Username, result.User!.Username);
            Assert.AreEqual(request.Email,    result.User.Email);
        }

        [TestMethod]
        public async Task RegisterAsync_DuplicateUser_ReturnsFailure()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Username = "existinguser",
                Email    = "existing@example.com",
                Password = "Test123!"
            };

            _mockAuthRepo.Setup(r => r.UserExistsAsync(request.Username, request.Email))
                .ReturnsAsync(true);

            // Act
            AuthResponseDTO result = await _authService.RegisterAsync(request);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("User with this username or email already exists", result.Message);
            Assert.IsNull(result.AccessToken);
        }

        [TestMethod]
        public async Task RegisterAsync_PasswordIsHashedWithBcrypt()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Username  = "hashtest",
                Email     = "hash@example.com",
                Password  = "PlainTextPass!",
                FirstName = "Hash",
                LastName  = "Test"
            };

            UserEntity? savedUser = null;

            _mockAuthRepo.Setup(r => r.UserExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            _mockAuthRepo.Setup(r => r.CreateUserAsync(It.IsAny<UserEntity>()))
                .Callback<UserEntity>(u => savedUser = u)
                .ReturnsAsync((UserEntity u) => { u.Id = 1; return u; });

            _mockAuthRepo.Setup(r => r.CreateRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .ReturnsAsync((RefreshTokenEntity rt) => rt);

            // Act
            await _authService.RegisterAsync(request);

            // Assert — hash must NOT be the plain text password
            Assert.IsNotNull(savedUser);
            Assert.AreNotEqual("PlainTextPass!", savedUser!.PasswordHash);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("PlainTextPass!", savedUser.PasswordHash));
        }

        // ==================== LoginAsync ====================

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            string passwordHash = BCrypt.Net.BCrypt.HashPassword("Test123!");

            var user = new UserEntity
            {
                Id           = 1,
                Username     = "testuser",
                Email        = "test@example.com",
                PasswordHash = passwordHash,
                IsActive     = true,
                Role         = "User"
            };

            var request = new LoginRequestDTO
            {
                UsernameOrEmail = "testuser",
                Password        = "Test123!"
            };

            _mockAuthRepo.Setup(r => r.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);
            _mockAuthRepo.Setup(r => r.UpdateUserAsync(It.IsAny<UserEntity>()))
                .Returns(Task.CompletedTask);
            _mockAuthRepo.Setup(r => r.RevokeAllUserTokensAsync(It.IsAny<long>(), It.IsAny<string?>()))
                .Returns(Task.CompletedTask);
            _mockAuthRepo.Setup(r => r.CreateRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .ReturnsAsync((RefreshTokenEntity rt) => rt);

            // Act
            AuthResponseDTO result = await _authService.LoginAsync(request);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Login successful", result.Message);
            Assert.IsNotNull(result.AccessToken);
            Assert.IsNotNull(result.RefreshToken);
            Assert.AreEqual(user.Username, result.User!.Username);
        }

        [TestMethod]
        public async Task LoginAsync_WrongPassword_ReturnsFailure()
        {
            // Arrange
            string passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!");

            var user = new UserEntity
            {
                Id           = 1,
                Username     = "testuser",
                PasswordHash = passwordHash,
                IsActive     = true
            };

            var request = new LoginRequestDTO
            {
                UsernameOrEmail = "testuser",
                Password        = "WrongPassword!"
            };

            _mockAuthRepo.Setup(r => r.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);
            _mockAuthRepo.Setup(r => r.UpdateUserAsync(It.IsAny<UserEntity>()))
                .Returns(Task.CompletedTask);

            // Act
            AuthResponseDTO result = await _authService.LoginAsync(request);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid username/email or password", result.Message);
            Assert.IsNull(result.AccessToken);
        }

        [TestMethod]
        public async Task LoginAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var request = new LoginRequestDTO
            {
                UsernameOrEmail = "nobody",
                Password        = "Test123!"
            };

            _mockAuthRepo.Setup(r => r.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync((UserEntity?)null);

            // Act
            AuthResponseDTO result = await _authService.LoginAsync(request);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid username/email or password", result.Message);
            Assert.IsNull(result.AccessToken);
        }

        [TestMethod]
        public async Task LoginAsync_InactiveUser_ReturnsFailure()
        {
            // Arrange
            string passwordHash = BCrypt.Net.BCrypt.HashPassword("Test123!");

            var user = new UserEntity
            {
                Id           = 1,
                Username     = "inactiveuser",
                PasswordHash = passwordHash,
                IsActive     = false    // deactivated
            };

            var request = new LoginRequestDTO
            {
                UsernameOrEmail = "inactiveuser",
                Password        = "Test123!"
            };

            _mockAuthRepo.Setup(r => r.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            // Act
            AuthResponseDTO result = await _authService.LoginAsync(request);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Account is deactivated. Please contact support.", result.Message);
            Assert.IsNull(result.AccessToken);
        }

        [TestMethod]
        public async Task LoginAsync_LockedOutUser_ReturnsFailure()
        {
            // Arrange
            var user = new UserEntity
            {
                Id           = 1,
                Username     = "lockeduser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                IsActive     = true,
                LockoutEnd   = DateTime.UtcNow.AddMinutes(10)   // still locked
            };

            var request = new LoginRequestDTO
            {
                UsernameOrEmail = "lockeduser",
                Password        = "Test123!"
            };

            _mockAuthRepo.Setup(r => r.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            // Act
            AuthResponseDTO result = await _authService.LoginAsync(request);

            // Assert
            Assert.IsFalse(result.Success);
            StringAssert.Contains(result.Message, "locked");
        }

        [TestMethod]
        public async Task LoginAsync_ExceedsMaxAttempts_SetsLockout()
        {
            // Arrange
            var user = new UserEntity
            {
                Id                  = 1,
                Username            = "willbelockeduser",
                PasswordHash        = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!"),
                IsActive            = true,
                FailedLoginAttempts = 4   // one more wrong attempt = lockout
            };

            var request = new LoginRequestDTO
            {
                UsernameOrEmail = "willbelockeduser",
                Password        = "WrongPassword!"
            };

            UserEntity? updatedUser = null;
            _mockAuthRepo.Setup(r => r.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);
            _mockAuthRepo.Setup(r => r.UpdateUserAsync(It.IsAny<UserEntity>()))
                .Callback<UserEntity>(u => updatedUser = u)
                .Returns(Task.CompletedTask);

            // Act
            await _authService.LoginAsync(request);

            // Assert — lockout should now be set
            Assert.IsNotNull(updatedUser);
            Assert.IsNotNull(updatedUser!.LockoutEnd);
            Assert.IsTrue(updatedUser.LockoutEnd > DateTime.UtcNow);
        }

        // ==================== RefreshTokenAsync ====================

        [TestMethod]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
        {
            // Arrange
            var user = new UserEntity
            {
                Id       = 1,
                Username = "testuser",
                Email    = "test@example.com",
                IsActive = true,
                Role     = "User"
            };

            var tokenEntity = new RefreshTokenEntity
            {
                Token     = "valid-refresh-token",
                UserId    = 1,
                User      = user,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                RevokedAt = null
            };

            _mockAuthRepo.Setup(r => r.GetRefreshTokenAsync("valid-refresh-token"))
                .ReturnsAsync(tokenEntity);
            _mockAuthRepo.Setup(r => r.RevokeRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .Returns(Task.CompletedTask);
            _mockAuthRepo.Setup(r => r.CreateRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .ReturnsAsync((RefreshTokenEntity rt) => rt);

            // Act
            AuthResponseDTO result = await _authService.RefreshTokenAsync("valid-refresh-token");

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Token refreshed successfully", result.Message);
            Assert.IsNotNull(result.AccessToken);
            Assert.IsNotNull(result.RefreshToken);
            // New token must be different from the old one
            Assert.AreNotEqual("valid-refresh-token", result.RefreshToken);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_ExpiredToken_ReturnsFailure()
        {
            // Arrange
            var tokenEntity = new RefreshTokenEntity
            {
                Token     = "expired-token",
                ExpiresAt = DateTime.UtcNow.AddDays(-1),  // already expired
                RevokedAt = null
            };

            _mockAuthRepo.Setup(r => r.GetRefreshTokenAsync("expired-token"))
                .ReturnsAsync(tokenEntity);

            // Act
            AuthResponseDTO result = await _authService.RefreshTokenAsync("expired-token");

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Refresh token has expired", result.Message);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_RevokedToken_ReturnsFailure()
        {
            // Arrange
            var tokenEntity = new RefreshTokenEntity
            {
                Token     = "revoked-token",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                RevokedAt = DateTime.UtcNow.AddHours(-1)   // revoked
            };

            _mockAuthRepo.Setup(r => r.GetRefreshTokenAsync("revoked-token"))
                .ReturnsAsync(tokenEntity);

            // Act
            AuthResponseDTO result = await _authService.RefreshTokenAsync("revoked-token");

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Refresh token has been revoked", result.Message);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_InvalidToken_ReturnsFailure()
        {
            // Arrange
            _mockAuthRepo.Setup(r => r.GetRefreshTokenAsync("ghost-token"))
                .ReturnsAsync((RefreshTokenEntity?)null);

            // Act
            AuthResponseDTO result = await _authService.RefreshTokenAsync("ghost-token");

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid refresh token", result.Message);
        }

        // ==================== LogoutAsync ====================

        [TestMethod]
        public async Task LogoutAsync_WithRefreshToken_RevokesToken()
        {
            // Arrange
            var tokenEntity = new RefreshTokenEntity
            {
                Token     = "token-to-revoke",
                UserId    = 1,
                RevokedAt = null
            };

            _mockAuthRepo.Setup(r => r.GetRefreshTokenAsync("token-to-revoke"))
                .ReturnsAsync(tokenEntity);
            _mockAuthRepo.Setup(r => r.RevokeRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .Returns(Task.CompletedTask);

            // Act
            AuthResponseDTO result = await _authService.LogoutAsync("token-to-revoke");

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Logged out successfully", result.Message);
            _mockAuthRepo.Verify(r =>
                r.RevokeRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()), Times.Once);
        }

        [TestMethod]
        public async Task LogoutAsync_WithUserId_RevokesAllUserTokens()
        {
            // Arrange
            long userId = 42;
            _mockAuthRepo.Setup(r => r.RevokeAllUserTokensAsync(userId, null))
                .Returns(Task.CompletedTask);

            // Act
            AuthResponseDTO result = await _authService.LogoutAsync(null, userId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Logged out successfully", result.Message);
            _mockAuthRepo.Verify(r =>
                r.RevokeAllUserTokensAsync(userId, null), Times.Once);
        }

        // ==================== GetUserProfileAsync ====================

        [TestMethod]
        public async Task GetUserProfileAsync_ValidId_ReturnsUserInfo()
        {
            // Arrange
            var user = new UserEntity
            {
                Id        = 1,
                Username  = "testuser",
                Email     = "test@example.com",
                FirstName = "Test",
                LastName  = "User",
                Role      = "User"
            };

            _mockAuthRepo.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

            // Act
            UserInfoDTO? result = await _authService.GetUserProfileAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1,              result!.Id);
            Assert.AreEqual("testuser",     result.Username);
            Assert.AreEqual("test@example.com", result.Email);
            Assert.AreEqual("Test",         result.FirstName);
            Assert.AreEqual("User",         result.LastName);
            Assert.AreEqual("User",         result.Role);
        }

        [TestMethod]
        public async Task GetUserProfileAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            _mockAuthRepo.Setup(r => r.GetUserByIdAsync(999))
                .ReturnsAsync((UserEntity?)null);

            // Act
            UserInfoDTO? result = await _authService.GetUserProfileAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        // ==================== BCrypt Hashing ====================

        [TestMethod]
        public void BCrypt_HashPassword_ProducesValidHash()
        {
            string password = "MySecurePassword!";
            string hash     = BCrypt.Net.BCrypt.HashPassword(password);

            Assert.IsNotNull(hash);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(password, hash));
        }

        [TestMethod]
        public void BCrypt_SamePassword_ProducesDifferentHashes()
        {
            string password = "MySecurePassword!";
            string hash1    = BCrypt.Net.BCrypt.HashPassword(password);
            string hash2    = BCrypt.Net.BCrypt.HashPassword(password);

            // BCrypt embeds a random salt, so hashes must differ
            Assert.AreNotEqual(hash1, hash2);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(password, hash1));
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(password, hash2));
        }

        [TestMethod]
        public void BCrypt_WrongPassword_FailsVerification()
        {
            string hash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!");
            Assert.IsFalse(BCrypt.Net.BCrypt.Verify("WrongPassword!", hash));
        }
    }
}
