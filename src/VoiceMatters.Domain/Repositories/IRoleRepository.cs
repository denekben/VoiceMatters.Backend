using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetAsyncByUserId(Guid userId);
    }
}
