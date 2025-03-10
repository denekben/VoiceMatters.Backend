using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.UseCases.Administration.Commands
{
    public sealed record UnblockPetition(Guid Id) : IRequest;

}
