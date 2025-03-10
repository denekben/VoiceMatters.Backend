using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface IPetitionRepository
    {
        Task<Petition?> GetAsync(Guid id);
        Task<Petition?> GetAsync(Guid id, PetitionIncludes petitionIncludes);
        Task DeleteAsync(Petition petition);
        Task UpdateAsync(Petition petition);
        Task AddAsync(Petition petition);
    }

    [Flags]
    public enum PetitionIncludes
    {
        Tags,
        PetitionTags,
        Images
    }
}
