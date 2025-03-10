using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.Configuration;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly int _keySize;
        private readonly int _iterations;
        private readonly HashAlgorithmName _hashAlgorithm;
        private readonly double _refreshTokenLifeTime;
        private readonly AppDbContext _context;

        public AuthService(IConfiguration configuration, AppDbContext context)
        {
            _refreshTokenLifeTime = Convert.ToInt32(configuration["JWT:RefreshTokenLifeTime"]);

            _context = context;

            var algorithmName = HashAlgorithmName.SHA512;
            _keySize = Convert.ToInt32(configuration["Hashing:KeySize"]);
            _iterations = Convert.ToInt32(configuration["Hashing:Iterations"]);
        }

        public async Task AssignUserToRole(Guid id, string roleName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id == id)
                ?? throw new BadRequestException($"Cannot find user with id {id}");
            var role = await _context.Roles.FirstOrDefaultAsync(r=>r.RoleName == roleName)
                ?? throw new BadRequestException($"Cannot find role with roleName {roleName}");

            user.RoleId = role.Id;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AppUser> CreateUserAsync(string firstName, string lastName, string? phone,
            string email, string hashedPassword, DateTime? birthDate, string? sex, string? imageURL, Guid roleId)
        {
            var user = AppUser.Create(firstName, lastName, phone, email, hashedPassword, birthDate, sex, imageURL, roleId)
                ?? throw new BadRequestException("Cannot create user");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Email == email)
                ?? throw new BadRequestException($"Cannot find user with email {email}");

            return user;
        }

        public string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(_keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                _iterations,
                _hashAlgorithm,
                _keySize);

            return $"{Convert.ToHexString(hash)}:{Convert.ToHexString(salt)}";
        }

        public bool VerifyPassword(string password, string storedHashWithSalt)
        {
            var parts = storedHashWithSalt.Split(':');
            if (parts.Length != 2)
                throw new BadRequestException("Incorect hash format");

            var storedHash = parts[0];
            var storedSalt = parts[1];

            var saltBytes = Convert.FromHexString(storedSalt);
            var hashToCheck = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                _iterations,
                _hashAlgorithm,
                _keySize);

            return storedHash.Equals(Convert.ToHexString(hashToCheck), StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> IsRefreshTokenValid(string email, string refreshToken)
        {
            var user = await GetUserByEmailAsync(email);
            return (user.RefreshToken == refreshToken && user.RefreshTokenExpires > DateTime.UtcNow);
        }

        public async Task SigninUserAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user.IsBlocked)
                throw new BadRequestException("User is blocked");
            
            if(!VerifyPassword(password, (string) user.PasswordHash))
                throw new BadRequestException("Cannot verify password");
        }

        public async Task UpdateRefreshToken(string email, string refreshToken)
        {
            var user = await GetUserByEmailAsync(email);

            var refreshTokenExpires = DateTime.UtcNow.AddDays(_refreshTokenLifeTime);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpires = refreshTokenExpires;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public Task<bool> UserExistsByEmail(string email)
        {
            return _context.Users.AnyAsync(u=>u.Email == email);
        }

        public async Task<bool> IsBlocked(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new BadRequestException($"Cannot find user {id}");

            return user.IsBlocked;
        }
    }
}
