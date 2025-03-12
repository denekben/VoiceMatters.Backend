using MediatR;
using Microsoft.EntityFrameworkCore;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Users.Queries;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Infrastructure.Queries.Users
{
    public sealed class GetUserPlatesByPetitionIdHandler : IRequestHandler<GetUserPlatesByPetitionId, List<ProfilePlateDto>?>
    {
        private readonly AppDbContext _context;

        public GetUserPlatesByPetitionIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProfilePlateDto>?> Handle(GetUserPlatesByPetitionId query, CancellationToken cancellationToken)
        {
            var users = _context.Users.AsNoTracking().Include(u=>u.PetitionsSignedByUser)
                .Where(u => u.PetitionsSignedByUser.Any(p=>p.PetitionId == query.PetitionId));

            int skipNumber = (query.PageNumber - 1) * query.PageSize;

            users = users.Skip(skipNumber).Take(query.PageSize);

            return await users.Select(u => u.AsProfilePlateDto()).ToListAsync();
        }
    }
}
