using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Shared.Services
{
    public class HttpContextService : IHttpContextService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HttpContextService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public Guid GetCurrentUserId()
        {
            var userIdString = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new AuthorizationException("Cannot find user");

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new AuthorizationException("User ID is not a valid guid");
            }

            return userId;
        }

        public string GetCurrentUserRoleName()
        {
            var roleName = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value
                ?? throw new AuthorizationException("Cannot find user's role");

            return roleName;
        }
    }
}
