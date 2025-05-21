using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface IAppUserRepository
    {
        Task<AppUser?> GetAsync(Guid id);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<AppUser?> GetAsync(Guid id, UserIncludes includes);
    }

    [Flags]
    public enum UserIncludes
    {
        Role = 1
    }
}
