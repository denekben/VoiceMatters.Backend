using Microsoft.AspNetCore.Authorization;

namespace VoiceMatters.WebUI.Policies
{
    public class IsNotBlockedRequirement : IAuthorizationRequirement
    {
        public bool IsBlocked { get; set; }
    }

}
