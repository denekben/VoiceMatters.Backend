using Microsoft.EntityFrameworkCore;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Infrastructure.Data;

namespace VoiceMatters.Infrastructure.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _context;

        public NewsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(News news)
        {
            await _context.News.AddAsync(news);
        }

        public async Task DeleteAsync(News news)
        {
            await _context.News.Where(n => n.Id == news.Id).ExecuteDeleteAsync();
        }

        public async Task<News?> GetAsync(Guid id)
        {
            return await _context.News.FirstOrDefaultAsync(n => n.Id == id);
        }
    }
}
