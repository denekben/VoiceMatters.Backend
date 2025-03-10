using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.UseCases.News.Commands
{
    public sealed record DeleteNews(Guid Id) : IRequest;
}
