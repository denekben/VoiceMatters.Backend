using MediatR;
using Microsoft.EntityFrameworkCore;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Users.Queries;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Infrastructure.Queries.Users
{
    public sealed class GetUserHandler : IRequestHandler<GetUser, ProfileDto?>
    {
        private readonly AppDbContext _context;

        public GetUserHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileDto?> Handle(GetUser query, CancellationToken cancellationToken)
        {
            return (await _context.Users.AsNoTracking()
                .Include(u => u.PetitionsCreatedByUser)
                .Include(u => u.PetitionsSignedByUser)
                .FirstOrDefaultAsync(u => u.Id == query.Id))
                ?.AsProfileDto();
        }
    }
}
