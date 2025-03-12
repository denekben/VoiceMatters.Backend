using DomainStatistic =  VoiceMatters.Domain.Entities.Statistic;

namespace VoiceMatters.Domain.Repositories
{
    public interface IStatisticRepository
    {
        Task<DomainStatistic?> GetAsync();
        Task UpdateAsync(DomainStatistic statistic);
    }
}
