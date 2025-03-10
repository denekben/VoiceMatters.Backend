using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.UseCases.Petitions.Commands
{
    public sealed record DeletePetition(Guid Id): IRequest;
}
