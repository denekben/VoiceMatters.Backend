using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.UseCases.Administration.Commands;
using VoiceMatters.Application.UseCases.Administration.Commands.Handlers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Administration;

public class BlockUserHandlerTests
{
    private readonly Mock<IAppUserRepository> _mockRepo = new();
    private readonly Mock<ILogger<BlockUserHandler>> _mockLogger = new();
    private readonly BlockUserHandler _handler;

    public BlockUserHandlerTests()
    {
        _handler = new BlockUserHandler(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new BlockUser(Guid.NewGuid());
        _mockRepo.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync((AppUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockRepo.Verify(x => x.UpdateAsync(It.IsAny<AppUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_BlocksUser()
    {
        // Arrange
        var user = new AppUser { Id = Guid.NewGuid(), IsBlocked = false };
        var command = new BlockUser(user.Id);

        _mockRepo.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(user);

        _mockRepo.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
            .Callback<AppUser>(u => user = u)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(user.IsBlocked);
        _mockRepo.Verify(x => x.UpdateAsync(user), Times.Once);
    }
}
