using Microsoft.AspNetCore.Authorization;
using VoiceMatters.Application.Services;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.WebUI.Policies
{
    public class IsNotBlockedHandler : AuthorizationHandler<IsNotBlockedRequirement>
    {
        private readonly IHttpContextService _httpContextService;
        private readonly IAuthService _authService;

        public IsNotBlockedHandler(IHttpContextService httpContextService, IAuthService authService)
        {
            _httpContextService = httpContextService;
            _authService = authService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsNotBlockedRequirement requirement)
        {
            var userId = _httpContextService.GetCurrentUserId();
            if (!await _authService.IsBlocked(userId))
            {
                context.Succeed(requirement);
            }
        }
    }
}
