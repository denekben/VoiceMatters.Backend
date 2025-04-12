using MediatR;
using Microsoft.EntityFrameworkCore;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Petitions.Queries;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Services;
using Sort = VoiceMatters.Shared.Helpers.SortType;

namespace VoiceMatters.Infrastructure.Queries.Petitions
{
    public sealed class GetPetitionsByUserIdHandler : IRequestHandler<GetPetitionsByUserId, List<PetitionDto>?>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextService _contextService;

        public GetPetitionsByUserIdHandler(AppDbContext context, IHttpContextService contextService)
        {
            _context = context;
            _contextService = contextService;
        }

        public async Task<List<PetitionDto>?> Handle(GetPetitionsByUserId query, CancellationToken cancellationToken)
        {
            Guid? currentUserId = null;
            try
            {
                currentUserId = _contextService.GetCurrentUserId();
            }
            catch (Exception) { }

            var userPetitions = _context.Petitions.AsNoTracking().Where(p => p.CreatorId == query.CreatorId);

            userPetitions = userPetitions.Where(p => EF.Functions.ILike(p.Title, $"%{query.SearchPhrase ?? string.Empty}%"));

            int skipNumber = (query.PageNumber - 1) * query.PageSize;
            userPetitions = userPetitions.Skip(skipNumber).Take(query.PageSize);

            if (query.TagIds != null && query.TagIds.Count != 0)
            {
                userPetitions = userPetitions.Include(p => p.PetitionTags).ThenInclude(pt => pt.Tag).Where(p => query.TagIds.All(tagId => p.PetitionTags.Any(pt => pt.Tag.Id == tagId)));
            }

            if (query.IncludeCompleted == Sort.Enable.ToString())
            {
                userPetitions = userPetitions.Where(p => p.IsCompleted == true);
            }
            else if (query.IncludeCompleted == Sort.Disable.ToString())
            {
                userPetitions = userPetitions.Where(p => p.IsCompleted == false);
            }

            if (query.SortBySignQuantityPerDay == Sort.Descending.ToString())
            {
                userPetitions = userPetitions.OrderByDescending(p => p.SignQuantityPerDay);
            }
            else if (query.SortBySignQuantityPerDay == Sort.Ascending.ToString())
            {
                userPetitions = userPetitions.OrderBy(p => p.SignQuantityPerDay);
            }

            if (query.SortBySignQuantity == Sort.Descending.ToString())
            {
                userPetitions = userPetitions.OrderByDescending(p => p.SignQuantity);
            }
            else if (query.SortBySignQuantity == Sort.Ascending.ToString())
            {
                userPetitions = userPetitions.OrderBy(p => p.SignQuantity);
            }

            if (query.SortByDate == Sort.Descending.ToString())
            {
                userPetitions = userPetitions.OrderByDescending(p => p.CreatedDate);
            }
            else if (query.SortBySignQuantity == Sort.Ascending.ToString())
            {
                userPetitions = userPetitions.OrderBy(p => p.CreatedDate);
            }

            AppUser? currentUser = null;
            if (currentUserId != null)
            {
                currentUser = await _context.Users.AsNoTracking().Include(u => u.PetitionsSignedByUser).Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == currentUserId);
            }
            if (currentUser?.Role.RoleName != Role.Admin.RoleName || !query.AllowBlocked)
                userPetitions = userPetitions.Where(p => !p.IsBlocked);

            userPetitions.Include(p => p.Images).Include(p => p.PetitionTags).ThenInclude(pt => pt.Tag).Include(p => p.Creator).Include(p => p.News);

            var signedPetitionIds = currentUser?.PetitionsSignedByUser?.Select(up => up.PetitionId).ToList() ?? new List<Guid>();

            return await userPetitions
                .Select(
                    p => p.AsDto(
                        currentUser != null &&
                        currentUser.PetitionsSignedByUser != null &&
                        signedPetitionIds.Contains(p.Id))
                ).ToListAsync();
        }
    }
}