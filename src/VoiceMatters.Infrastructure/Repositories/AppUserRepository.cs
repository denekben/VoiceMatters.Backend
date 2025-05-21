using Microsoft.EntityFrameworkCore;
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
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<AppUser?> GetAsync(Guid id, UserIncludes includes)
        {
            var query = _context.Users.Where(u => u.Id == id);
            if (includes.HasFlag(UserIncludes.Role))
                query = query.Include(u => u.Role);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
