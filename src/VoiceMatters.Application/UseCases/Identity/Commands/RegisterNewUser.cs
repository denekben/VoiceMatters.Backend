using MediatR;
using Microsoft.AspNetCore.Http;
using VoiceMatters.Shared.DTOs;

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
        IFormFile? Image) : IRequest<TokensDto?>;
}
