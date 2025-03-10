using VoiceMatters.Domain.Entities;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.Services
{
    public interface IAuthService
    {
        Task AssignUserToRole(Guid id, string roleName);
        Task<AppUser> CreateUserAsync(string firstName, string lastName, string? phone, 
            string email, string hashedPassword, DateTime? birthDate, string? sex, string? imageURL, Guid roleId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHashWithSalt);
        Task<bool> UserExistsByEmail(string email);
        Task<AppUser> GetUserByEmailAsync(string email);
        Task SigninUserAsync(string email, string password);

        Task UpdateRefreshToken(string email, string refreshToken);
        Task<bool> IsRefreshTokenValid(string email, string refreshToken);
        Task<bool> IsBlocked(Guid id);
    }
}
  