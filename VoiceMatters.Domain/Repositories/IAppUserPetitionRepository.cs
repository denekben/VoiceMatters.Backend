using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Entities.Pivots;

namespace VoiceMatters.Domain.Repositories
{
    public interface IAppUserPetitionRepository
    {
        Task<AppUserSignedPetition?> GetAsync(Guid petitionId, Guid signerId);
        Task AddAsync(AppUserSignedPetition userSignedPetition);
    }
}
