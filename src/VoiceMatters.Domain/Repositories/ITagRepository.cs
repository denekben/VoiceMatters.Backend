using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetTagByNameAsync(string tagName);
        Task<Tag?> GetByNameNoTrackingAsync(string tagName);
        Task AddAsync(Tag tag);
    }
}
