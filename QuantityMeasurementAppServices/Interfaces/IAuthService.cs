using QuantityMeasurementAppModels.DTOs;

namespace QuantityMeasurementAppServices.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request, string? ipAddress = null);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, string? ipAddress = null);
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
        Task<AuthResponseDTO> LogoutAsync(string? refreshToken = null, long? userId = null, string? ipAddress = null);
        Task<UserInfoDTO?> GetUserProfileAsync(long userId);
    }
}