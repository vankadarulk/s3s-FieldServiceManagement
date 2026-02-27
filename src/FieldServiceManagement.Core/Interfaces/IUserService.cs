using FieldServiceManagement.Core.DTOs;

namespace FieldServiceManagement.Core.Interfaces;

/// <summary>Defines operations for managing users.</summary>
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(UserCreateDto dto);
    Task<UserDto?> UpdateAsync(Guid id, UserCreateDto dto);
    Task<UserDto?> AuthenticateAsync(string email, string password);
}
