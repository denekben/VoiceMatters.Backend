using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Users.Queries;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Infrastructure.Queries.Users
{
    internal sealed class GetUserPlatesHandler : IRequestHandler<GetUserPlates, List<ProfilePlateDto>?>
    {
        private readonly AppDbContext _context;

        public GetUserPlatesHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProfilePlateDto>?> Handle(GetUserPlates query, CancellationToken cancellationToken)
        {
            var users = _context.Users.AsNoTracking()
                .Where(u=>(
                EF.Functions.ILike(u.FirstName, $"{query.SearchPhrase ?? string.Empty}") ||
                EF.Functions.ILike(u.LastName, $"{query.SearchPhrase ?? string.Empty}")));

            int skipNumber = (query.PageNumber - 1) * query.PageSize;

            users = users.Skip(skipNumber).Take(query.PageSize);

            return await users.Select(u => u.AsProfilePlateDto()).ToListAsync();
        }
    }
}