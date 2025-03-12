using MediatR;

namespace VoiceMatters.Application.UseCases.Administration.Commands
{
    public sealed record AssignUserToRole(Guid Id, string RoleName) : IRequest;
}
