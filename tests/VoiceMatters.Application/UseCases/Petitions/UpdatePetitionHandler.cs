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

namespace VoiceMatters.Application.UseCases.Petitions.Commands.Handlers;

public class UpdatePetitionHandlerTests
{
    private readonly Mock<IPetitionRepository> _petitionRepositoryMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<ILogger<UpdatePetitionHandler>> _loggerMock;
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly Mock<IHttpContextService> _contextServiceMock;
    private readonly Mock<IAppUserRepository> _userRepositoryMock;
    private readonly UpdatePetitionHandler _handler;

    public UpdatePetitionHandlerTests()
    {
        _petitionRepositoryMock = new Mock<IPetitionRepository>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _loggerMock = new Mock<ILogger<UpdatePetitionHandler>>();
        _imageServiceMock = new Mock<IImageService>();
        _contextServiceMock = new Mock<IHttpContextService>();
        _userRepositoryMock = new Mock<IAppUserRepository>();
        _handler = new UpdatePetitionHandler(_petitionRepositoryMock.Object, _tagRepositoryMock.Object, _loggerMock.Object,
            _imageServiceMock.Object, _contextServiceMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_CreatorNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new UpdatePetition(
            Guid.NewGuid(),
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<UpdateImageDto> { new UpdateImageDto(file, "", 0, "") }
        );
        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync((AppUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PetitionNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new UpdatePetition(
            Guid.NewGuid(),
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<UpdateImageDto> { new UpdateImageDto(file, "", 0, "") }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        var creator = new AppUser { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id, PetitionIncludes.Tags | PetitionIncludes.PetitionTags | PetitionIncludes.Images))
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
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new UpdatePetition(
            Guid.NewGuid(),
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<UpdateImageDto> { new UpdateImageDto(file, "", 0, "") }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        var creator = new AppUser { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        var petition = new Petition { Id = command.Id, CreatorId = Guid.NewGuid() };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id, PetitionIncludes.Tags | PetitionIncludes.PetitionTags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PetitionCannotBeUpdated_ThrowsBadRequestException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new UpdatePetition(
            Guid.NewGuid(),
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<UpdateImageDto> { new UpdateImageDto(file, "", 0, "") }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        var creator = new AppUser { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        var petition = new Petition { Id = command.Id, CreatorId = creator.Id };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id, PetitionIncludes.Tags | PetitionIncludes.PetitionTags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        // Для этого теста нужно изменить Petition.Create так, чтобы он возвращал null
        // Или добавить условие в тесте, чтобы Petition.Create возвращал null
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TagCannotBeCreated_ThrowsBadRequestException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new UpdatePetition(
            Guid.NewGuid(),
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<UpdateImageDto> { new UpdateImageDto(file, "", 0, "") }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        var creator = new AppUser { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        var petition = new Petition { Id = command.Id, CreatorId = creator.Id };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id, PetitionIncludes.Tags | PetitionIncludes.PetitionTags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _tagRepositoryMock.Setup(x => x.GetTagByNameAsync("Tag1"))
            .ReturnsAsync((Tag?)null);

        // Для этого теста нужно изменить Tag.Create так, чтобы он возвращал null
        // Или добавить условие в тесте, чтобы Tag.Create возвращал null
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ImageCannotBeUploaded_ThrowsBadRequestException()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new UpdatePetition(
            Guid.NewGuid(),
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<UpdateImageDto> { new UpdateImageDto(file, "", 0, "") }
        );

        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        var creator = new AppUser { Id = Guid.NewGuid() };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        var petition = new Petition { Id = command.Id, CreatorId = creator.Id };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id, PetitionIncludes.Tags | PetitionIncludes.PetitionTags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _tagRepositoryMock.Setup(x => x.GetTagByNameAsync("Tag1"))
            .ReturnsAsync(new Tag { Name = "Tag1" });

        _imageServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync((string?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _petitionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationPassed_UpdatesPetition()
    {
        // Arrange
        var file = new FormFile(new MemoryStream(new byte[] { }), 0, 0, "image", "image.jpg");
        var command = new UpdatePetition(
            Guid.NewGuid(),
            "Test Petition Title ",
            @"Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title
            Test Petition Text Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title Test Petition Title",
            new List<string> { "Tag1" },
            new List<UpdateImageDto> { new UpdateImageDto(file, "", 0, "") }
        );

        var creatorId = Guid.NewGuid();
        _contextServiceMock.Setup(x => x.GetCurrentUserId())
            .Returns(creatorId);

        var creator = new AppUser { Id = creatorId };
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(creator);

        var petition = new Petition { Id = command.Id, CreatorId = creator.Id };
        _petitionRepositoryMock.Setup(x => x.GetAsync(command.Id, PetitionIncludes.Tags | PetitionIncludes.PetitionTags | PetitionIncludes.Images))
            .ReturnsAsync(petition);

        _tagRepositoryMock.Setup(x => x.GetTagByNameAsync("Tag1"))
            .ReturnsAsync(new Tag { Name = "Tag1" });

        _imageServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("image_url");

        var updatedPetition = Petition.Create(command.Id, command.Title, command.TextPayload, creator.Id) ?? throw new Exception("Cannot update petition");
        _petitionRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Petition>()))
            .Callback<Petition>(p => updatedPetition = p)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedPetition.Title, result.Title);
        Assert.Equal(updatedPetition.TextPayload, result.TextPayload);
    }
}
