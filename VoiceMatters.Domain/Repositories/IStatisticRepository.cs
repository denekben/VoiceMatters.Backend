using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Entities;
using DomainStatistic =  VoiceMatters.Domain.Entities.Statistic;

namespace VoiceMatters.Domain.Repositories
{
    public interface IStatisticRepository
    {
        Task<DomainStatistic?> GetAsync();
        Task UpdateAsync(DomainStatistic statistic);
    }
}
