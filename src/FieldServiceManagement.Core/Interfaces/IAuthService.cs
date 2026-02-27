using FieldServiceManagement.Core.DTOs;

namespace FieldServiceManagement.Core.Interfaces;

/// <summary>Defines authentication operations.</summary>
public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string userId);
    Task<UserDto?> GetCurrentUserAsync(string userId);
}
