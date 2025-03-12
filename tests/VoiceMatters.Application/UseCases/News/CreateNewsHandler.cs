using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.UseCases.News.Commands;
using VoiceMatters.Application.UseCases.News.Commands.Handlers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;
using DomainNews = VoiceMatters.Domain.Entities.News;

namespace VoiceMatters.Application.UseCases.Administration;

public class CreateNewsHandlerTests
{
    private readonly Mock<ILogger<CreateNewsHandler>> _loggerMock;
    private readonly Mock<IHttpContextService> _contextServiceMock;
    private readonly Mock<IPetitionRepository> _petitionRepositoryMock;
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly CreateNewsHandler _handler;

    public CreateNewsHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateNewsHandler>>();
        _contextServiceMock = new Mock<IHttpContextService>();
        _petitionRepositoryMock = new Mock<IPetitionRepository>();
        _newsRepositoryMock = new Mock<INewsRepository>();
        _handler = new CreateNewsHandler(_loggerMock.Object, _contextServiceMock.Object, _petitionRepositoryMock.Object, _newsRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_PetitionNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new CreateNews(Guid.NewGuid(), "Test News");

        _petitionRepositoryMock.Setup(x => x.GetAsync(command.PetitionId, PetitionIncludes.Tags | PetitionIncludes.Images))
            .ReturnsAsync((Petition?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _newsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DomainNews>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AuthorizationAccessError_ThrowsBadRequestException()
    {
        // Arrange
        var command = new CreateNews(Guid.NewGuid(), "Test News");

        var petition = new Petition { Id = command.PetitionId, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.PetitionId, PetitionIncludes.Tags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _newsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DomainNews>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PetitionUncompleted_ThrowsBadRequestException()
    {
        // Arrange
        var command = new CreateNews(Guid.NewGuid(), "Test News");

        var petition = new Petition { Id = command.PetitionId, IsCompleted = false, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.PetitionId, PetitionIncludes.Tags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns((Guid)petition.CreatorId);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _newsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DomainNews>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PetitionBlocked_ThrowsBadRequestException()
    {
        // Arrange
        var command = new CreateNews(Guid.NewGuid(), "Test News");

        var petition = new Petition { Id = command.PetitionId, IsBlocked = true, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.PetitionId, PetitionIncludes.Tags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns((Guid)petition.CreatorId);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _newsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DomainNews>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NewsCannotBeCreated_ThrowsBadRequestException()
    {
        // Arrange
        var command = new CreateNews(Guid.NewGuid(), "Test News");

        var petition = new Petition { Id = command.PetitionId, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.PetitionId, PetitionIncludes.Tags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns((Guid)petition.CreatorId);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _newsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DomainNews>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_ReturnsNewsDto()
    {
        // Arrange
        var command = new CreateNews(Guid.NewGuid(), "Test News Title");

        var petition = new Petition { Id = command.PetitionId, CreatorId = Guid.NewGuid(), IsCompleted = true, IsBlocked = false };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.PetitionId, PetitionIncludes.Tags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns((Guid)petition.CreatorId);

        var news = DomainNews.Create(command.Title, command.PetitionId) ?? throw new Exception("Cannot create news");
        _newsRepositoryMock.Setup(x => x.AddAsync(It.IsAny<DomainNews>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }
}
