using VoiceMatters.Domain.Entities.Pivots;

namespace VoiceMatters.Domain.Repositories
{
    public interface IAppUserPetitionRepository
    {
        Task<AppUserSignedPetition?> GetAsync(Guid petitionId, Guid signerId);
        Task AddAsync(AppUserSignedPetition userSignedPetition);
    }
}
