using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface IImageRepository
    {
        Task<List<Image>?> GetByPetitionIdAsync(Guid id);
    }
}
