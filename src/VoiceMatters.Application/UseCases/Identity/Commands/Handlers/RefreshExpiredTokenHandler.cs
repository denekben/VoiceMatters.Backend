using MediatR;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Identity.Commands.Handlers
{
    public sealed class RefreshExpiredTokenHandler : IRequestHandler<RefreshExpiredToken, string?>
    {
        private readonly IAppUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _contextAccessor;

        public RefreshExpiredTokenHandler(
            IAppUserRepository userRepository, ITokenService tokenService,
            IHttpContextAccessor contextAccessor)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _contextAccessor = contextAccessor;
        }

        public async Task<string?> Handle(RefreshExpiredToken command, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            var user = await _userRepository.GetAsync(userId, UserIncludes.Role)
                ?? throw new AuthorizationException($"Cannot find user {userId}");

            if (user.RefreshTokenExpires < DateTime.UtcNow)
                throw new AuthorizationException("Refresh token expired");
            var decodedRefreshToken = Uri.UnescapeDataString(command.RefreshToken)
                .Replace(" ", "+");
            if (user.RefreshToken != decodedRefreshToken)
                throw new AuthorizationException("Incorrect refresh token");

            var token = _tokenService.GenerateAccessToken(user.Id, user.LastName, user.Email, user.Role.RoleName);

            return token;
        }

        public Guid GetCurrentUserId()
        {
            var authHeader = _contextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();

                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

                if (userIdClaim != null)
                {
                    return Guid.Parse(userIdClaim);
                }
            }
            var userIdString = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new AuthorizationException("Cannot find user");

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new AuthorizationException("User ID is not a valid guid");
            }

            return userId;
        }
    }
}
