using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface IAppUserRepository
    {
        Task<AppUser?> GetAsync(Guid id);
        Task<AppUser?> GetByEmailAsync(string email);
        Task UpdateAsync(AppUser user);
    }
}
