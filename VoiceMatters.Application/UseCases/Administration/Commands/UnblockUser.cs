using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.UseCases.Administration.Commands
{
    public sealed record UnblockUser(Guid Id) : IRequest;

}
