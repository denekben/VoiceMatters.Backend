using System.Security.Claims;

namespace VoiceMatters.Application.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(Guid id, string lastname, string email, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
