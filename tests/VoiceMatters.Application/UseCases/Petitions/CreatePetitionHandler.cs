using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;
using Stat = VoiceMatters.Domain.Entities.Statistic;

namespace VoiceMatters.Application.UseCases.Petitions.Commands.Handlers;

public class CreatePetitionHandlerTests
{
    private readonly Mock<IPetitionRepository> _petitionRepositoryMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<ILogger<CreatePetitionHandler>> _loggerMock;
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly Mock<IHttpContextService> _contextServiceMock;
    private readonly Mock<IStatisticRepository> _statisticRepositoryMock;
    private readonly Mock<INotificationService> _notificationsMock;
    private readonly Mock<IAppUserRepository> _userRepositoryMock;
    private readonly CreatePetitionHandler _handler;

    public CreatePetitionHandlerTests()
    {
        _petitionRepositoryMock = new Mock<IPetitionRepository>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _loggerMock = new Mock<ILogger<CreatePetitionHandler>>();
        _imageServiceMock = new Mock<IImageService>();
        _contextServiceMock = new Mock<IHttpContextService>();
        _statisticRepositoryMock = new Mock<IStatisticRepository>();
        _notificationsMock = new Mock<INotificationService>();
        _userRepositoryMock = new Mock<IAppUserRepository>();
        _handler = new CreatePetitionHandler(_petitionRepositoryMock.Object, _tagRepositoryMock.Object, _loggerMock.Object,
            _imageServiceMock.Object, _contextServiceMock.Object, _statisticRepositoryMock.Object, _notificationsMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_CreatorNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new CreatePetition(
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<CreateImageDto> { new CreateImageDto(file, "", 0) }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync((AppUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PetitionCannotBeCreated_ThrowsBadRequestException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new CreatePetition(
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<CreateImageDto> { new CreateImageDto(file, "", 0) }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        var creator = new AppUser { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TagCannotBeCreated_ThrowsBadRequestException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new CreatePetition(
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<CreateImageDto> { new CreateImageDto(file, "", 0) }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        var creator = new AppUser { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        _tagRepositoryMock.Setup(x => x.GetTagByNameAsync("Tag1"))
            .ReturnsAsync((Tag?)null);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ImageCannotBeUploaded_ThrowsBadRequestException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new CreatePetition(
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<CreateImageDto> { new CreateImageDto(file, "", 0) }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        var creator = new AppUser { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        _tagRepositoryMock.Setup(x => x.GetTagByNameAsync("Tag1"))
            .ReturnsAsync(new Tag { Name = "Tag1" });

        _imageServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync((string?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_CreatesPetition()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new CreatePetition(
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<CreateImageDto> { new CreateImageDto(file, "", 0) }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        var creator = new AppUser { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        var tag = new Tag { Name = "Tag1" };
        _tagRepositoryMock.Setup(x => x.GetTagByNameAsync("Tag1"))
            .ReturnsAsync(tag);

        _imageServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("image_url");

        var petition = Petition.Create(command.Title, command.TextPayload, creator.Id) ?? throw new Exception("Cannot create petition");
        _petitionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Petition>()))
            .Callback<Petition>(p => petition = p)
            .Returns(Task.CompletedTask);

        var stats = new Stat();
        _statisticRepositoryMock.Setup(x => x.GetAsync())
            .ReturnsAsync(stats);

        _statisticRepositoryMock.Setup(x => x.UpdateAsync(stats))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(petition.Title, result.Title);
        Assert.Equal(petition.TextPayload, result.TextPayload);
    }
}
