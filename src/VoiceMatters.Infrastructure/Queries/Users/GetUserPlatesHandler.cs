﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.UseCases.Users.Queries;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Infrastructure.Queries.Users
{
    public sealed class GetUserPlatesHandler : IRequestHandler<GetUserPlates, List<ProfilePlateDto>?>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextService _contextService;

        public GetUserPlatesHandler(AppDbContext context, IHttpContextService contextService)
        {
            _context = context;
            _contextService = contextService;
        }

        public async Task<List<ProfilePlateDto>?> Handle(GetUserPlates query, CancellationToken cancellationToken)
        {
            Guid? userId = null;
            AppUser? currentUser = null;
            try
            {
                userId = _contextService.GetCurrentUserId();
            }
            catch (Exception) { }

            if (userId != null)
            {
                currentUser = await _context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            }

            var users = _context.Users.AsNoTracking().Include(u => u.Role)
                .Where(u => (
                EF.Functions.ILike(u.FirstName + " " + u.LastName, $"%{query.SearchPhrase ?? string.Empty}%") ||
                EF.Functions.ILike(u.LastName + " " + u.FirstName, $"%{query.SearchPhrase ?? string.Empty}%")));

            if (currentUser?.Role.RoleName != Role.Admin.RoleName || !query.AllowBlocked)
                users = users.Where(u => !u.IsBlocked);

            int skipNumber = (query.PageNumber - 1) * query.PageSize;

            users = users.Skip(skipNumber).Take(query.PageSize);

            return await users.Select(u => u.AsProfilePlateDto()).ToListAsync();
        }
    }
}