using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.News.Commands
{
    public sealed record UpdateNews(Guid Id, string Title) : IRequest<NewsDto?>;
}
