using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.UseCases.Administration.Commands;
using VoiceMatters.Application.UseCases.Administration.Commands.Handlers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Administration;

public class UnblockUserHandlerTests
{
    private readonly Mock<IAppUserRepository> _mockRepo = new();
    private readonly Mock<ILogger<UnblockUserHandler>> _mockLogger = new();
    private readonly UnblockUserHandler _handler;

    public UnblockUserHandlerTests()
    {
        _handler = new UnblockUserHandler(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new UnblockUser(Guid.NewGuid());
        _mockRepo.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync((AppUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockRepo.Verify(x => x.UpdateAsync(It.IsAny<AppUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_UnblocksUser()
    {
        // Arrange
        var user = new AppUser { Id = Guid.NewGuid(), IsBlocked = true };
        var command = new UnblockUser(user.Id);

        _mockRepo.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(user);

        _mockRepo.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
            .Callback<AppUser>(u => user = u)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(user.IsBlocked);
        _mockRepo.Verify(x => x.UpdateAsync(user), Times.Once);
    }
}
