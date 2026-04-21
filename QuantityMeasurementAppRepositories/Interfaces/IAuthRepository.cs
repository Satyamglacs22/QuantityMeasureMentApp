using QuantityMeasurementAppModels.Entities;

namespace QuantityMeasurementAppRepositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserEntity?> GetUserByIdAsync(long id);
        Task<UserEntity?> GetUserByUsernameAsync(string username);
        Task<UserEntity?> GetUserByEmailAsync(string email);
        Task<UserEntity?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<UserEntity> CreateUserAsync(UserEntity user);
        Task UpdateUserAsync(UserEntity user);
        Task<bool> UserExistsAsync(string username, string email);
        Task<int> GetTotalUserCountAsync();
        Task<RefreshTokenEntity> CreateRefreshTokenAsync(RefreshTokenEntity refreshToken);
        Task<RefreshTokenEntity?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshTokenEntity refreshToken);
        Task RevokeAllUserTokensAsync(long userId, string? revokedByIp = null);
    }
}