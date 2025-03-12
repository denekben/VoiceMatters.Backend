using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Application.UseCases.Petitions.Commands.Handlers;

public class CompletePetitionHandlerTests
{
    private readonly Mock<ILogger<CompletePetitionHandler>> _loggerMock;
    private readonly Mock<IHttpContextService> _contextServiceMock;
    private readonly Mock<IPetitionRepository> _petitionRepositoryMock;
    private readonly CompletePetitionHandler _handler;

    public CompletePetitionHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CompletePetitionHandler>>();
        _contextServiceMock = new Mock<IHttpContextService>();
        _petitionRepositoryMock = new Mock<IPetitionRepository>();
        _handler = new CompletePetitionHandler(_loggerMock.Object, _contextServiceMock.Object, _petitionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_PetitionNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new CompletePetition(Guid.NewGuid());

        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync((Petition?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AuthorizationAccessError_ThrowsBadRequestException()
    {
        // Arrange
        var command = new CompletePetition(Guid.NewGuid());

        var petition = new Petition { Id = command.Id, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PetitionBlocked_ThrowsBadRequestException()
    {
        // Arrange
        var command = new CompletePetition(Guid.NewGuid());

        var petition = new Petition { Id = command.Id, CreatorId = Guid.NewGuid(), IsBlocked = true };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns((Guid)petition.CreatorId);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_CompletesPetition()
    {
        // Arrange
        var command = new CompletePetition(Guid.NewGuid());

        var petition = new Petition { Id = command.Id, CreatorId = Guid.NewGuid(), IsBlocked = false };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns((Guid)petition.CreatorId);

        _petitionRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Petition>()))
            .Callback<Petition>(p => petition = p)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(petition.IsCompleted);
        Assert.NotNull(petition.CompletedDate);
    }
}
