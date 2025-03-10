using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Application.UseCases.Statistic.Queries;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Infrastructure.Data;
using DomainStatistic = VoiceMatters.Domain.Entities.Statistic;

namespace VoiceMatters.Infrastructure.Queries.Statistic
{
    internal sealed class GetStatisticHandler : IRequestHandler<GetStatistic, DomainStatistic?>
    {
        private readonly AppDbContext _context;

        public GetStatisticHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DomainStatistic?> Handle(GetStatistic query, CancellationToken cancellationToken)
        {
            return await _context.Statistic.AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
