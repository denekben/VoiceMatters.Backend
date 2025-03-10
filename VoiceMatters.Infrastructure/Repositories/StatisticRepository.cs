using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Infrastructure.Data;
using DomainStatistic = VoiceMatters.Domain.Entities.Statistic;


namespace VoiceMatters.Infrastructure.Repositories
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly AppDbContext _context;

        public StatisticRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DomainStatistic?> GetAsync()
        {
            return await _context.Statistic.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(DomainStatistic stats)
        {
            _context.Statistic.Update(stats);
            await _context.SaveChangesAsync();
        }
    }
}
