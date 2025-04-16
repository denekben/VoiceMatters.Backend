using MediatR;
using Microsoft.EntityFrameworkCore;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Petitions.Queries;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Infrastructure.Queries.Petitions
{
    public sealed class GetPetitionHandler : IRequestHandler<GetPetition, PetitionDto?>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextService _contextService;

        public GetPetitionHandler(AppDbContext context, IHttpContextService contextService)
        {
            _context = context;
            _contextService = contextService;
        }

        public async Task<PetitionDto?> Handle(GetPetition query, CancellationToken cancellationToken)
        {
            Guid? userId = null;
            try
            {
                userId = _contextService.GetCurrentUserId();
            }
            catch (Exception) { }

            var petition = _context.Petitions.AsNoTracking().Where(p => p.Id == query.Id);
            AppUser? currentUser = null;

            if (userId != null)
            {
                currentUser = await _context.Users.AsNoTracking().Include(u => u.PetitionsSignedByUser).Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            }
            if (currentUser?.Role.RoleName != Role.Admin.RoleName || !query.AllowBlocked)
                petition = petition.Where(p => !p.IsBlocked);

            petition = petition.Include(p => p.Images).Include(p => p.PetitionTags).ThenInclude(pt => pt.Tag).Include(p => p.Creator).Include(p => p.News);

            var petitionEntity = await petition.FirstOrDefaultAsync();

            return (petitionEntity)?.AsDto(
                currentUser != null &&
                currentUser.PetitionsSignedByUser != null &&
                currentUser.PetitionsSignedByUser.Any(up => up.PetitionId == query.Id));
        }
    }
}
