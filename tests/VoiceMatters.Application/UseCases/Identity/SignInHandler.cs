using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.Services;
using VoiceMatters.Application.UseCases.Identity.Commands;
using VoiceMatters.Application.UseCases.Identity.Commands.Handlers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Administration;

public class SignInHandlerTests
{
    private readonly Mock<IAuthService> _mockAuthService = new();
    private readonly Mock<ITokenService> _mockTokenService = new();
    private readonly Mock<ILogger<SignInHandler>> _mockLogger = new();
    private readonly Mock<IRoleRepository> _mockRoleRepository = new();
    private readonly SignInHandler _handler;

    public SignInHandlerTests()
    {
        _handler = new SignInHandler(_mockAuthService.Object, _mockTokenService.Object, _mockLogger.Object, _mockRoleRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidationPassed_ReturnsTokenDto()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";

        _mockAuthService.Setup(x => x.SigninUserAsync(email, password))
            .Returns(Task.CompletedTask);

        var user = new AppUser { Id = Guid.NewGuid(), Email = email, LastName = "Lastname" };
        _mockAuthService.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(user);

        var refreshToken = "refresh_token";
        _mockTokenService.Setup(x => x.GenerateRefreshToken())
            .Returns(refreshToken);

        _mockAuthService.Setup(x => x.UpdateRefreshToken(email, refreshToken))
            .Returns(Task.CompletedTask);

        var role = new Role { RoleName = "RoleName" };
        _mockRoleRepository.Setup(x => x.GetAsync(user.Id))
            .ReturnsAsync(role);

        var accessToken = "access_token";
        _mockTokenService.Setup(x => x.GenerateAccessToken(user.Id, user.LastName, email, role.RoleName))
            .Returns(accessToken);

        var command = new SignIn(email, password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessToken, result.AccessToken);
        Assert.Equal(refreshToken, result.RefreshToken);

        _mockLogger.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"User {email} signed in")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()!),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_RoleNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";

        _mockAuthService.Setup(x => x.SigninUserAsync(email, password))
            .Returns(Task.CompletedTask);

        var user = new AppUser { Id = Guid.NewGuid(), Email = email };
        _mockAuthService.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(user);

        var refreshToken = "refresh_token";
        _mockTokenService.Setup(x => x.GenerateRefreshToken())
            .Returns(refreshToken);

        _mockAuthService.Setup(x => x.UpdateRefreshToken(email, refreshToken))
            .Returns(Task.CompletedTask);

        _mockRoleRepository.Setup(x => x.GetAsync(user.Id))
            .ReturnsAsync((Role?)null);

        var command = new SignIn(email, password);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_GenerateAccessTokenFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";

        _mockAuthService.Setup(x => x.SigninUserAsync(email, password))
            .Returns(Task.CompletedTask);

        var user = new AppUser { Id = Guid.NewGuid(), Email = email, LastName = "Lastname" };
        _mockAuthService.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(user);

        var refreshToken = "refresh_token";
        _mockTokenService.Setup(x => x.GenerateRefreshToken())
            .Returns(refreshToken);

        _mockAuthService.Setup(x => x.UpdateRefreshToken(email, refreshToken))
            .Returns(Task.CompletedTask);

        var role = new Role { RoleName = "RoleName" };
        _mockRoleRepository.Setup(x => x.GetAsync(user.Id))
            .ReturnsAsync(role);

        _mockTokenService.Setup(x => x.GenerateAccessToken(user.Id, user.LastName, email, role.RoleName))
            .Returns((string?)null);

        var command = new SignIn(email, password);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockLogger.Verify(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception?, string>>()), Times.Never);
    }
}
