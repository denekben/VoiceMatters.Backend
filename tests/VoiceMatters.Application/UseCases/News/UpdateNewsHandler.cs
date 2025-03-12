using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.UseCases.News.Commands;
using VoiceMatters.Application.UseCases.News.Commands.Handlers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;
using DomainNews = VoiceMatters.Domain.Entities.News;

namespace VoiceMatters.Application.UseCases.News;

public class UpdateNewsHandlerTests
{
    private readonly Mock<ILogger<UpdateNewsHandler>> _loggerMock;
    private readonly Mock<IHttpContextService> _contextServiceMock;
    private readonly Mock<IPetitionRepository> _petitionRepositoryMock;
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly UpdateNewsHandler _handler;

    public UpdateNewsHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateNewsHandler>>();
        _contextServiceMock = new Mock<IHttpContextService>();
        _petitionRepositoryMock = new Mock<IPetitionRepository>();
        _newsRepositoryMock = new Mock<INewsRepository>();
        _handler = new UpdateNewsHandler(_loggerMock.Object, _contextServiceMock.Object, _petitionRepositoryMock.Object, _newsRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_NewsNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new UpdateNews(Guid.NewGuid(), "Test news title");

        _newsRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync((DomainNews?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<PetitionIncludes>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PetitionNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var command = new UpdateNews(Guid.NewGuid(), "Test news title");

        var news = new DomainNews { Id = command.Id, PetitionId = Guid.NewGuid() };
        _newsRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(news);

        _petitionRepositoryMock.Setup(x => x.GetAsync(news.PetitionId, PetitionIncludes.Images | PetitionIncludes.Tags))
            .ReturnsAsync((Petition?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _newsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DomainNews>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AuthorizationAccessError_ThrowsBadRequestException()
    {
        // Arrange
        var command = new UpdateNews(Guid.NewGuid(), "Test news title");

        var news = new DomainNews { Id = command.Id, PetitionId = Guid.NewGuid() };
        _newsRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(news);

        var petition = new Petition { Id = news.PetitionId, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(news.PetitionId, PetitionIncludes.Images | PetitionIncludes.Tags))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _newsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DomainNews>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_ReturnsNewsDto()
    {
        // Arrange
        var command = new UpdateNews(Guid.NewGuid(), "Test news title");

        var news = new DomainNews { Id = command.Id, PetitionId = Guid.NewGuid(), Title = "Old Test news title" };
        _newsRepositoryMock.Setup(x => x.GetAsync(command.Id))
            .ReturnsAsync(news);

        var petition = new Petition { Id = news.PetitionId, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(news.PetitionId, PetitionIncludes.Images | PetitionIncludes.Tags))
            .ReturnsAsync(petition);

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns((Guid)petition.CreatorId);

        var updatedNews = DomainNews.Create(news.Id, news.Title, news.PetitionId) ?? throw new Exception("Cannot update news");
        _newsRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<DomainNews>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
    }
}
