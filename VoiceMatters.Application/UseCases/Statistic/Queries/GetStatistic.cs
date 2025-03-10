using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Entities;
using DomainStatistic = VoiceMatters.Domain.Entities.Statistic;

namespace VoiceMatters.Application.UseCases.Statistic.Queries
{
    public sealed record GetStatistic: IRequest<DomainStatistic?>;
}
