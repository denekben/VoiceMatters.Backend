using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.News.Queries;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Helpers;
using Sort = VoiceMatters.Shared.Helpers.SortType;

namespace VoiceMatters.Infrastructure.Queries.News
{
    internal sealed class GetNewsHandler : IRequestHandler<GetNews, List<NewsDto>?>
    {
        private readonly AppDbContext _context;

        public GetNewsHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NewsDto>?> Handle(GetNews query, CancellationToken cancellationToken)
        {
            var news = _context.News.AsNoTracking().Where(n => EF.Functions.ILike(n.Title, $"%{query.SearchPhrase ?? string.Empty}%"));

            int skipNumber = (query.PageNumber - 1) * query.PageSize;

            news = news.Skip(skipNumber).Take(query.PageSize);

            news = news
                .Include(n => n.Petition)
                .ThenInclude(p => p.Tags)
                .Include(n => n.Petition)
                .ThenInclude(p => p.Images);

            if (query.SortBySignQuantityPerDay == Sort.Descending)
            {
                news = news.OrderByDescending(n => n.Petition.SignQuantityPerDay);
            }
            else if (query.SortBySignQuantityPerDay == Sort.Ascending)
            {
                news = news.OrderBy(n => n.Petition.SignQuantityPerDay);
            }

            if (query.SortBySignQuantity == Sort.Descending)
            {
                news = news.OrderByDescending(n => n.Petition.SignQuantity);
            }
            else if (query.SortBySignQuantity == Sort.Ascending)
            {
                news = news.OrderBy(n => n.Petition.SignQuantity);
            }

            if (query.SortByCompleteDate == Sort.Descending)
            {
                news = news.OrderByDescending(n => n.Petition.CompletedDate);
            }
            else if (query.SortByCompleteDate == Sort.Ascending)
            {
                news = news.OrderBy(n => n.Petition.CompletedDate);
            }

            return await news.Select(n=>n.AsDto()).ToListAsync();
        }
    }
}