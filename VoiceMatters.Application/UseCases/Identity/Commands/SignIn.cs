using VoiceMatters.Shared.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.UseCases.Identity.Commands
{
    public sealed record SignIn(string Email, string Password) : IRequest<TokensDto?>;
}
