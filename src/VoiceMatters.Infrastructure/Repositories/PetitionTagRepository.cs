using Microsoft.EntityFrameworkCore;
using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Infrastructure.Data;

namespace VoiceMatters.Infrastructure.Repositories
{
    public class PetitionTagRepository : IPetitionTagRepository
    {
        private readonly AppDbContext _context;

        public PetitionTagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PetitionTag petitionTag)
        {
            await _context.PetitionTags.AddAsync(petitionTag);
        }

        public async Task DeleteByPetitionIdAsync(Guid id)
        {
            await _context.PetitionTags.Where(pt => pt.PetitionId == id).ExecuteDeleteAsync();
        }
    }
}
