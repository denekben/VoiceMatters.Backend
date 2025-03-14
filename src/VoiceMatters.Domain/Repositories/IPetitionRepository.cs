using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface IPetitionRepository
    {
        Task<Petition?> GetAsync(Guid id);
        Task<Petition?> GetAsync(Guid id, PetitionIncludes petitionIncludes, TrackingType trackingType = TrackingType.Tracking);
        Task DeleteAsync(Petition petition);
        Task UpdateAsync(Petition petition);
        Task AddAsync(Petition petition);
    }

    [Flags]
    public enum PetitionIncludes
    {
        Tags,
        Images
    }

    public enum TrackingType
    {
        NoTracking,
        Tracking
    }
}
