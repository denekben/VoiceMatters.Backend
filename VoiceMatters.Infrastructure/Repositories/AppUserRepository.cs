using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Infrastructure.Data;

namespace VoiceMatters.Infrastructure.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly AppDbContext _context;

        public AppUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser?> GetAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u=>u.Id == id);
        }

        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateAsync(AppUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
