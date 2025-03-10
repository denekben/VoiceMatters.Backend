using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.UseCases.Identity.Commands
{
    public sealed record RefreshExpiredToken(string AccessToken, string RefreshToken) : IRequest<string?>;
}
