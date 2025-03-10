namespace VoiceMatters.Shared.Services
{
    public interface IHttpContextService
    {
        Guid GetCurrentUserId();
        string GetCurrentUserRoleName();
    }
}
