using MediatR;

namespace VoiceMatters.Application.UseCases.Administration.Commands
{
    public sealed record UnblockUser(Guid Id) : IRequest;

}
