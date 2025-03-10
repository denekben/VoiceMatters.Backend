using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Petitions.Queries;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Infrastructure.Queries.Petitions
{
    internal sealed class GetPetitionHandler : IRequestHandler<GetPetition, PetitionDto?>
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
                currentUser = await _context.Users.AsNoTracking().Include(u=>u.PetitionsSignedByUser).FirstOrDefaultAsync(u=>u.Id == userId);
            }

            petition = petition.Include(p => p.Images).Include(p => p.Tags).Include(p=>p.Creator);

            return (await petition.FirstOrDefaultAsync())?.AsDto(
                currentUser != null &&
                currentUser.PetitionsSignedByUser != null &&
                currentUser.PetitionsSignedByUser.Any(up => up.PetitionId == query.Id));
        }
    }
}
