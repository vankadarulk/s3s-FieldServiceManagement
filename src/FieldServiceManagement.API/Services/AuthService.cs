using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FieldServiceManagement.API.Data;
using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FieldServiceManagement.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public AuthService(AppDbContext context, IConfiguration configuration, IUserService userService)
    {
        _context = context;
        _configuration = configuration;
        _userService = userService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userService.AuthenticateAsync(request.Email, request.Password);
        if (user == null) return null;

        var dbUser = await _context.Users.FindAsync(user.Id);
        if (dbUser == null) return null;

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryInMinutes", 60);
        var refreshExpiryDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryInDays", 7);

        dbUser.RefreshToken = refreshToken;
        dbUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshExpiryDays);
        dbUser.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiry = DateTime.UtcNow.AddMinutes(expiryMinutes),
            User = user
        };
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.RefreshToken == refreshToken &&
            u.RefreshTokenExpiryTime > DateTime.UtcNow &&
            u.IsActive);

        if (user == null) return null;

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        var token = GenerateJwtToken(userDto);
        var newRefreshToken = GenerateRefreshToken();
        var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryInMinutes", 60);
        var refreshExpiryDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryInDays", 7);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshExpiryDays);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = newRefreshToken,
            Expiry = DateTime.UtcNow.AddMinutes(expiryMinutes),
            User = userDto
        };
    }

    public async Task LogoutAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var guid)) return;
        var user = await _context.Users.FindAsync(guid);
        if (user == null) return;
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<UserDto?> GetCurrentUserAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var guid)) return null;
        return await _userService.GetByIdAsync(guid);
    }

    private string GenerateJwtToken(UserDto user)
    {
        var secret = _configuration["JwtSettings:Secret"] ?? "DefaultSecretKey32CharactersLong!";
        var issuer = _configuration["JwtSettings:Issuer"] ?? "FieldServiceManagement";
        var audience = _configuration["JwtSettings:Audience"] ?? "FieldServiceManagement";
        var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryInMinutes", 60);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
