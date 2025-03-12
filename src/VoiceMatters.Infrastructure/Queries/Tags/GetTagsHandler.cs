using MediatR;
using Microsoft.EntityFrameworkCore;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Tags.Queries;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Infrastructure.Queries.Tags
{
    public sealed class GetTagsHandler : IRequestHandler<GetTags, List<TagDto>?>
    {
        private readonly AppDbContext _context;

        public GetTagsHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TagDto>?> Handle(GetTags query, CancellationToken cancellationToken)
        {
            var tags = _context.Tags.AsNoTracking().Where(t=>EF.Functions.ILike(t.Name, $"{query.SearchPhrase ?? string.Empty}"));

            int skipNumber = (query.PageNumber - 1) * query.PageSize;

            tags = tags.Skip(skipNumber).Take(query.PageSize);

            return await tags.Select(t=>t.AsDto()).ToListAsync();
        }
    }
}