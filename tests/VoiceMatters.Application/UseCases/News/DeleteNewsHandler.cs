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

public class DeleteNewsHandlerTests
{
    private readonly Mock<ILogger<DelegatingHandler>> _loggerMock;
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly Mock<IHttpContextService> _contextServiceMock;
    private readonly Mock<IPetitionRepository> _petitionRepositoryMock;
    private readonly DeleteNewsHandler _handler;

    public DeleteNewsHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DelegatingHandler>>();
        _newsRepositoryMock = new Mock<INewsRepository>();
        _contextServiceMock = new Mock<IHttpContextService>();
        _petitionRepositoryMock = new Mock<IPetitionRepository>();
        _handler = new DeleteNewsHandler(_loggerMock.Object, _newsRepositoryMock.Object, _contextServiceMock.Object, _petitionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_NewsNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new DeleteNews(Guid.NewGuid());

        _newsRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync((DomainNews?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PetitionNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new DeleteNews(Guid.NewGuid());

        var news = new DomainNews { Id = command.Id, PetitionId = Guid.NewGuid() };
        _newsRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(news);

        _petitionRepositoryMock.Setup(x => x.GetAsync(news.PetitionId))
            .ReturnsAsync((Petition?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _newsRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<DomainNews>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AuthorizationAccessError_ThrowsBadRequestException()
    {
        // Arrange
        var command = new DeleteNews(Guid.NewGuid());

        var news = new DomainNews { Id = command.Id, PetitionId = Guid.NewGuid() };
        _newsRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(news);

        var petition = new Petition { Id = news.PetitionId, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(news.PetitionId))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _newsRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<DomainNews>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_DeletesNews()
    {
        // Arrange
        var command = new DeleteNews(Guid.NewGuid());

        var news = new DomainNews { Id = command.Id, PetitionId = Guid.NewGuid() };
        _newsRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(news);

        var petition = new Petition { Id = news.PetitionId, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(news.PetitionId))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns((Guid)petition.CreatorId);

        _newsRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<DomainNews>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _newsRepositoryMock.Verify(x => x.DeleteAsync(news), Times.Once);
    }
}
