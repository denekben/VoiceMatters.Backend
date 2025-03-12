using VoiceMatters.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoiceMatters.Application.UseCases.Identity.Commands
{
    public sealed record RegisterNewUser(
        string FirstName,
        string LastName,
        string? Phone,
        string Password,
        string Email,
        DateTime? DateOfBirth,
        string? Sex,
        IFormFile Image) : IRequest<TokensDto?>;
}
