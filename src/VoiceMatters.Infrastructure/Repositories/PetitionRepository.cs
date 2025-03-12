using Microsoft.EntityFrameworkCore;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Infrastructure.Data;

namespace VoiceMatters.Infrastructure.Repositories
{
    public class PetitionRepository : IPetitionRepository
    {
        private readonly AppDbContext _context;

        public PetitionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Petition petition)
        {
            await _context.Petitions.AddAsync(petition);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Petition petition)
        {
            _context.Petitions.Remove(petition);
            await _context.SaveChangesAsync();
        }

        public async Task<Petition?> GetAsync(Guid id)
        {
            return await _context.Petitions.FirstOrDefaultAsync(p=>p.Id == id);
        }

        public async Task<Petition?> GetAsync(Guid id, PetitionIncludes petitionIncludes)
        {
            var query = _context.Petitions.Where(p=>p.Id == id);

            if (petitionIncludes == PetitionIncludes.Tags)
                query = query.Include(p => p.Tags);
            if (petitionIncludes == PetitionIncludes.Images)
                query = query.Include(p => p.Images);
            if (petitionIncludes == PetitionIncludes.PetitionTags)
                query = query.Include(p => p.PetitionTags);

            return await query.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Petition petition)
        {
            _context.Petitions.Update(petition);
            await _context.SaveChangesAsync();
        }
    }
}
