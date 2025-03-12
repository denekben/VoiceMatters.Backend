using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.Services;
using VoiceMatters.Application.UseCases.Identity.Commands;
using VoiceMatters.Application.UseCases.Identity.Commands.Handlers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using Stat = VoiceMatters.Domain.Entities.Statistic;

namespace VoiceMatters.Application.UseCases.Administration;

public class RegisterNewUserHandlerTests
{
    private readonly Mock<IAuthService> _mockAuthService = new();
    private readonly Mock<ILogger<RegisterNewUserHandler>> _mockLogger = new();
    private readonly Mock<ITokenService> _mockTokenService = new();
    private readonly Mock<IRoleRepository> _mockRoleRepository = new();
    private readonly Mock<IImageService> _mockImageService = new();
    private readonly Mock<IStatisticRepository> _mockStatisticRepository = new();
    private readonly Mock<INotificationService> _mockNotificationService = new();
    private readonly RegisterNewUserHandler _handler;

    public RegisterNewUserHandlerTests()
    {
        _handler = new RegisterNewUserHandler(_mockAuthService.Object, _mockLogger.Object, _mockTokenService.Object,
            _mockRoleRepository.Object, _mockImageService.Object, _mockStatisticRepository.Object, _mockNotificationService.Object);
    }

    [Fact]
    public async Task Handle_ValidationPassed_ReturnsTokenDto()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var phone = "+1234567890";
        var password = "password123";
        var email = "john@example.com";
        var dateOfBirth = DateTime.Now.AddYears(-20);
        var sex = "Male";
        var image = new byte[] { };

        var role = new Role { Id = Guid.NewGuid(), RoleName = Role.User.RoleName };
        _mockRoleRepository.Setup(x => x.GetByNameAsync(Role.User.RoleName))
            .ReturnsAsync(role);

        var hashedPassword = "hashed_password";
        _mockAuthService.Setup(x => x.HashPassword(password))
            .Returns(hashedPassword);

        using var stream = new MemoryStream(image);
        var formFile = new FormFile(stream, 0, image.Length, "image", "image.jpg");

        var imageURL = "image_url";
        _mockImageService.Setup(x => x.UploadFileAsync(formFile))
            .ReturnsAsync(imageURL);

        var userId = Guid.NewGuid();
        var user = new AppUser
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone
        };

        _mockAuthService.Setup(x => x.CreateUserAsync(firstName, lastName, phone, email, hashedPassword, dateOfBirth, sex, imageURL, role.Id))
            .ReturnsAsync(user);

        var refreshToken = "refresh_token";
        _mockTokenService.Setup(x => x.GenerateRefreshToken())
            .Returns(refreshToken);

        _mockAuthService.Setup(x => x.UpdateRefreshToken(email, refreshToken))
            .Returns(Task.CompletedTask);

        var accessToken = "access_token";
        _mockTokenService.Setup(x => x.GenerateAccessToken(userId, lastName, email, Role.User.RoleName))
            .Returns(accessToken);

        var stats = new Stat();
        _mockStatisticRepository.Setup(x => x.GetAsync())
            .ReturnsAsync(stats);

        _mockStatisticRepository.Setup(x => x.UpdateAsync(stats))
            .Returns(Task.CompletedTask);

        var command = new RegisterNewUser(firstName, lastName, phone, password, email, dateOfBirth, sex, formFile);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessToken, result.AccessToken);
        Assert.Equal(refreshToken, result.RefreshToken);
    }

    [Fact]
    public async Task Handle_RoleNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var phone = "+1234567890";
        var password = "password123";
        var email = "john@example.com";
        var dateOfBirth = DateTime.Now.AddYears(-20);
        var sex = "Male";
        var image = new byte[] { };

        _mockRoleRepository.Setup(x => x.GetByNameAsync(Role.User.RoleName))
            .ReturnsAsync((Role?)null);

        using var stream = new MemoryStream(image);
        var formFile = new FormFile(stream, 0, image.Length, "image", "image.jpg");

        var command = new RegisterNewUser(firstName, lastName, phone, password, email, dateOfBirth, sex, formFile);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockAuthService.Verify(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_GenerateAccessTokenFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var phone = "+1234567890";
        var password = "password123";
        var email = "john@example.com";
        var dateOfBirth = DateTime.Now.AddYears(-20);
        var sex = "Male";
        var image = new byte[] { };

        var role = new Role { Id = Guid.NewGuid(), RoleName = Role.User.RoleName };
        _mockRoleRepository.Setup(x => x.GetByNameAsync(Role.User.RoleName))
            .ReturnsAsync(role);

        var hashedPassword = "hashed_password";
        _mockAuthService.Setup(x => x.HashPassword(password))
            .Returns(hashedPassword);

        using var stream = new MemoryStream(image);
        var formFile = new FormFile(stream, 0, image.Length, "image", "image.jpg");

        var imageURL = "image_url";
        _mockImageService.Setup(x => x.UploadFileAsync(formFile))
            .ReturnsAsync(imageURL);

        var userId = Guid.NewGuid();
        var user = new AppUser
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone
        };

        _mockAuthService.Setup(x => x.CreateUserAsync(firstName, lastName, phone, email, hashedPassword, dateOfBirth, sex, imageURL, role.Id))
            .ReturnsAsync(user);

        var refreshToken = "refresh_token";
        _mockTokenService.Setup(x => x.GenerateRefreshToken())
            .Returns(refreshToken);

        _mockAuthService.Setup(x => x.UpdateRefreshToken(email, refreshToken))
            .Returns(Task.CompletedTask);

        _mockTokenService.Setup(x => x.GenerateAccessToken(userId, lastName, email, Role.User.RoleName))
            .Returns((string?)null);

        var command = new RegisterNewUser(firstName, lastName, phone, password, email, dateOfBirth, sex, formFile);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockLogger.Verify(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception?, string>>()), Times.Never);
    }
}
