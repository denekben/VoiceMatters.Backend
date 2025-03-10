using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Petitions.Queries;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Services;
using Sort = VoiceMatters.Shared.Helpers.SortType;

namespace VoiceMatters.Infrastructure.Queries.Petitions
{
    internal sealed class GetCurrentUserPetitionsHandler : IRequestHandler<GetCurrentUserPetitions, List<PetitionDto>?>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextService _contextService;

        public GetCurrentUserPetitionsHandler(AppDbContext context, IHttpContextService contextService)
        {
            _context = context;
            _contextService = contextService;
        }

        public async Task<List<PetitionDto>?> Handle(GetCurrentUserPetitions query, CancellationToken cancellationToken)
        {
            var userId = _contextService.GetCurrentUserId();
            var userPetitions = _context.Petitions.AsNoTracking().Where(p=>p.CreatorId == userId);

            userPetitions = userPetitions.Where(p => EF.Functions.ILike(p.Title, $"{query.SearchPhrase ?? string.Empty}"));

            int skipNumber = (query.PageNumber - 1) * query.PageSize;
            userPetitions = userPetitions.Skip(skipNumber).Take(query.PageSize);

            if(query.TagIds != null && query.TagIds.Count != 0)
            {
                userPetitions = userPetitions.Include(p => p.Tags).Where(p => query.TagIds.All(tagId => p.Tags.Any(t => t.Id == tagId)));
            }

            if(query.IncludeCompleted == Sort.Enable)
            {
                userPetitions = userPetitions.Where(p=>p.IsCompleted == true);
            }
            else if (query.IncludeCompleted == Sort.Disable)
            {
                userPetitions = userPetitions.Where(p=>p.IsCompleted == false);
            }

            if(query.SortBySignQuantityPerDay == Sort.Descending)
            {
                userPetitions = userPetitions.OrderByDescending(p=>p.SignQuantityPerDay);
            }
            else if (query.SortBySignQuantityPerDay == Sort.Ascending)
            {
                userPetitions = userPetitions.OrderBy(p=>p.SignQuantityPerDay);
            }

            if(query.SortBySignQuantity == Sort.Descending)
            {
                userPetitions = userPetitions.OrderByDescending(p => p.SignQuantity);
            }
            else if (query.SortBySignQuantity == Sort.Ascending)
            {
                userPetitions = userPetitions.OrderBy(p => p.SignQuantity);
            }

            if(query.SortByDate == Sort.Descending)
            {
                userPetitions = userPetitions.OrderByDescending(p => p.CreatedDate);
            }
            else if (query.SortBySignQuantity == Sort.Ascending)
            {
                userPetitions = userPetitions.OrderBy(p => p.CreatedDate);
            }

            userPetitions = userPetitions.Include(p => p.Images).Include(p => p.Tags).Include(p=>p.Creator);

            return await userPetitions.Select(p=>p.AsDto(p.CreatorId == userId)).ToListAsync();
        }
    }
}