using MediatR;
using Microsoft.EntityFrameworkCore;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Users.Queries;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Infrastructure.Queries.Users
{
    public sealed class GetUserHandler : IRequestHandler<GetUser, ProfileDto?>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextService _contextService;

        public GetUserHandler(AppDbContext context, IHttpContextService contextService)
        {
            _context = context;
            _contextService = contextService;
        }

        public async Task<ProfileDto?> Handle(GetUser query, CancellationToken cancellationToken)
        {
            Guid? userId = null;
            AppUser? currentUser = null;
            var users = _context.Users.AsNoTracking().Where(u => u.Id == query.Id);
            try
            {
                userId = _contextService.GetCurrentUserId();
            }
            catch (Exception) { }

            if (userId != null)
            {
                currentUser = await _context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            }
            if (currentUser?.Role.RoleName != Role.Admin.RoleName || !query.AllowBlocked)
                users = users.Where(u => !u.IsBlocked);

            var userEntities = await users.Include(u => u.PetitionsCreatedByUser).Include(u => u.PetitionsSignedByUser).FirstOrDefaultAsync();

            return (userEntities?.AsProfileDto());
        }
    }
}
