using MediatR;
using DomainStatistic = VoiceMatters.Domain.Entities.Statistic;

namespace VoiceMatters.Application.UseCases.Statistic.Queries
{
    public sealed record GetStatistic: IRequest<DomainStatistic?>;
}
