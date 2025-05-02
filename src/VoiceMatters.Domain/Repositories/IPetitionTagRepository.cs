using VoiceMatters.Domain.Entities.Pivots;

namespace VoiceMatters.Domain.Repositories
{
    public interface IPetitionTagRepository
    {
        Task DeleteByPetitionIdAsync(Guid id);
        Task AddAsync(PetitionTag petitionTag);
    }
}
