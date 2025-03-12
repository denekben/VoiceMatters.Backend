using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.UseCases.Administration.Commands;
using VoiceMatters.Application.UseCases.Administration.Commands.Handlers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Administration;

public class UnblockPetitionHandlerTests
{
    private readonly Mock<IPetitionRepository> _mockRepo = new();
    private readonly Mock<ILogger<UnblockPetitionHandler>> _mockLogger = new();
    private readonly UnblockPetitionHandler _handler;

    public UnblockPetitionHandlerTests()
    {
        _handler = new UnblockPetitionHandler(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_PetitionNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new UnblockPetition(Guid.NewGuid());
        _mockRepo.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync((Petition?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockRepo.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_UnblocksPetition()
    {
        // Arrange
        var petition = new Petition { Id = Guid.NewGuid(), IsBlocked = true };
        var command = new UnblockPetition(petition.Id);

        _mockRepo.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(petition);

        _mockRepo.Setup(x => x.UpdateAsync(It.IsAny<Petition>()))
            .Callback<Petition>(p => petition = p)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(petition.IsBlocked);
        _mockRepo.Verify(x => x.UpdateAsync(petition), Times.Once);
    }
}
