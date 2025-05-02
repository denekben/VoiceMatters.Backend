using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface INewsRepository
    {
        Task<News?> GetAsync(Guid id);
        Task AddAsync(News news);
        Task DeleteAsync(News news);
    }
}
