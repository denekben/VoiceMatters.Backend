using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;
using Stat = VoiceMatters.Domain.Entities.Statistic;

namespace VoiceMatters.Application.UseCases.Petitions.Commands.Handlers;

public class SignPetitionHandlerTests
{
    private readonly Mock<IAppUserPetitionRepository> _userPetitionRepositoryMock;
    private readonly Mock<IPetitionRepository> _petitionRepositoryMock;
    private readonly Mock<ILogger<SignPetitionHandler>> _loggerMock;
    private readonly Mock<IHttpContextService> _contextServiceMock;
    private readonly Mock<IStatisticRepository> _statisticRepositoryMock;
    private readonly Mock<INotificationService> _notificationsMock;
    private readonly SignPetitionHandler _handler;

    public SignPetitionHandlerTests()
    {
        _userPetitionRepositoryMock = new Mock<IAppUserPetitionRepository>();
        _petitionRepositoryMock = new Mock<IPetitionRepository>();
        _loggerMock = new Mock<ILogger<SignPetitionHandler>>();
        _contextServiceMock = new Mock<IHttpContextService>();
        _statisticRepositoryMock = new Mock<IStatisticRepository>();
        _notificationsMock = new Mock<INotificationService>();
        _handler = new SignPetitionHandler(_userPetitionRepositoryMock.Object, _loggerMock.Object, _contextServiceMock.Object,
            _petitionRepositoryMock.Object, _statisticRepositoryMock.Object, _notificationsMock.Object);
    }

    [Fact]
    public async Task Handle_PetitionNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new SignPetition(Guid.NewGuid());

        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync((Petition?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _userPetitionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AppUserSignedPetition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PetitionAlreadySigned_ThrowsBadRequestException()
    {
        // Arrange
        var command = new SignPetition(Guid.NewGuid());

        var petition = new Petition { Id = command.Id };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(petition);

        var signerId = Guid.NewGuid();
        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(signerId);

        _userPetitionRepositoryMock.Setup(x => x.GetAsync(petition.Id, signerId))
            .ReturnsAsync(new AppUserSignedPetition { PetitionId = petition.Id, SignerId = signerId });

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _userPetitionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AppUserSignedPetition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_SignsPetition()
    {
        // Arrange
        var command = new SignPetition(Guid.NewGuid());

        var petition = new Petition { Id = command.Id };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(petition);

        var signerId = Guid.NewGuid();
        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(signerId);

        _userPetitionRepositoryMock.Setup(x => x.GetAsync(petition.Id, signerId))
            .ReturnsAsync((AppUserSignedPetition?)null);

        _userPetitionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AppUserSignedPetition>()))
            .Returns(Task.CompletedTask);

        var stats = new Stat();
        _statisticRepositoryMock.Setup(x => x.GetAsync())
            .ReturnsAsync(stats);

        _statisticRepositoryMock.Setup(x => x.UpdateAsync(stats))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userPetitionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AppUserSignedPetition>()), Times.Once);

        _loggerMock.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Petition signed")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()!),
            Times.Once
        );
    }

}
