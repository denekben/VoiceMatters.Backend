using MediatR;
using Microsoft.EntityFrameworkCore;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Users.Queries;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Infrastructure.Queries.Users
{
    public sealed class GetCurrentUserHandler : IRequestHandler<GetCurrentUser, ProfileDto?>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextService _contextService;

        public GetCurrentUserHandler(AppDbContext context, IHttpContextService contextService)
        {
            _context = context;
            _contextService = contextService;
        }

        public async Task<ProfileDto?> Handle(GetCurrentUser query, CancellationToken cancellationToken)
        {
            var userId = _contextService.GetCurrentUserId();

            return (await _context.Users.AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.PetitionsCreatedByUser)
                .Include(u => u.PetitionsSignedByUser)
                .FirstOrDefaultAsync(u => u.Id == userId))
                ?.AsProfileDto();
        }
    }
}
