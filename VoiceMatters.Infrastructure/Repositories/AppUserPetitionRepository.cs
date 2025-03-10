using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Infrastructure.Data;

namespace VoiceMatters.Infrastructure.Repositories
{
    public class AppUserPetitionRepository : IAppUserPetitionRepository
    {
        private readonly AppDbContext _context;

        public AppUserPetitionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AppUserSignedPetition userSignedPetition)
        {
            await _context.AppUserSignedPetitions.AddAsync(userSignedPetition);
            await _context.SaveChangesAsync();
        }

        public async Task<AppUserSignedPetition?> GetAsync(Guid petitionId, Guid signerId)
        {
            return await _context.AppUserSignedPetitions.FirstOrDefaultAsync(up=>(up.PetitionId == petitionId && up.SignerId == signerId));
        }
    }
}
