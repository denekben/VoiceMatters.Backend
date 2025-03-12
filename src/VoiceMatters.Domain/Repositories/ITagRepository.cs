using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetTagByNameAsync(string tagName);
    }
}
