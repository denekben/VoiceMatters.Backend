﻿using Microsoft.EntityFrameworkCore;
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
        }

        public async Task DeleteAsync(Petition petition)
        {
            await _context.Petitions.Where(p => p.Id == petition.Id).ExecuteDeleteAsync();
        }

        public async Task<Petition?> GetAsync(Guid id)
        {
            return await _context.Petitions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Petition?> GetAsync(Guid id, PetitionIncludes petitionIncludes, TrackingType trType)
        {
            var query = _context.Petitions.Where(p => p.Id == id);

            if (trType == TrackingType.NoTracking)
                query = query.AsNoTracking();

            if (petitionIncludes.HasFlag(PetitionIncludes.Tags))
                query = query.Include(p => p.PetitionTags).ThenInclude(pt => pt.Tag);
            if (petitionIncludes.HasFlag(PetitionIncludes.Images))
                query = query.Include(p => p.Images);

            return await query.FirstOrDefaultAsync();
        }
    }
}
