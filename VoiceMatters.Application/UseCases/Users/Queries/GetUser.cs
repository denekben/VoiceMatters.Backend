using VoiceMatters.Shared.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.UseCases.Users.Queries
{
    public sealed record GetUser(Guid Id) : IRequest<ProfileDto>;
}
