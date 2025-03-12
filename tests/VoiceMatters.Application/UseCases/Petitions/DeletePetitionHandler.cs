using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;
using Stat = VoiceMatters.Domain.Entities.Statistic;

namespace VoiceMatters.Application.UseCases.Petitions.Commands.Handlers;

public class DeletePetitionHandlerTests
{
    private readonly Mock<IPetitionRepository> _petitionRepositoryMock;
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly Mock<ILogger<DeletePetitionHandler>> _loggerMock;
    private readonly Mock<IHttpContextService> _contextServiceMock;
    private readonly Mock<IStatisticRepository> _statisticRepositoryMock;
    private readonly Mock<INotificationService> _notificationsMock;
    private readonly DeletePetitionHandler _handler;

    public DeletePetitionHandlerTests()
    {
        _petitionRepositoryMock = new Mock<IPetitionRepository>();
        _imageServiceMock = new Mock<IImageService>();
        _loggerMock = new Mock<ILogger<DeletePetitionHandler>>();
        _contextServiceMock = new Mock<IHttpContextService>();
        _statisticRepositoryMock = new Mock<IStatisticRepository>();
        _notificationsMock = new Mock<INotificationService>();
        _handler = new DeletePetitionHandler(_petitionRepositoryMock.Object, _loggerMock.Object, _imageServiceMock.Object,
            _contextServiceMock.Object, _statisticRepositoryMock.Object, _notificationsMock.Object);
    }

    [Fact]
    public async Task Handle_PetitionNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new DeletePetition(Guid.NewGuid());

        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id, PetitionIncludes.Images))
            .ReturnsAsync((Petition?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AuthorizationAccessError_ThrowsBadRequestException()
    {
        // Arrange
        var command = new DeletePetition(Guid.NewGuid());

        var petition = new Petition { Id = command.Id, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id, PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_DeletesPetition()
    {
        // Arrange
        var command = new DeletePetition(Guid.NewGuid());

        var petition = new Petition { Id = command.Id, CreatorId = Guid.NewGuid(), Images = new List<Image> { new Image { Uuid = "image-uuid" } } };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id, PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns((Guid)petition.CreatorId);

        _imageServiceMock.Setup(x => x.DeleteByUuidAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _petitionRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Petition>()))
            .Returns(Task.CompletedTask);

        var stats = new Stat();
        _statisticRepositoryMock.Setup(x => x.GetAsync())
            .ReturnsAsync(stats);

        _statisticRepositoryMock.Setup(x => x.UpdateAsync(stats))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _petitionRepositoryMock.Verify(x => x.DeleteAsync(petition), Times.Once);
    }
}
