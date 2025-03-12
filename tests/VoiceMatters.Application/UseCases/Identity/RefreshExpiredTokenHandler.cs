using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using VoiceMatters.Application.Services;
using VoiceMatters.Application.UseCases.Identity.Commands;
using VoiceMatters.Application.UseCases.Identity.Commands.Handlers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Administration;

public class RefreshExpiredTokenHandlerTests
{
    private readonly Mock<IAuthService> _mockAuthService = new();
    private readonly Mock<ITokenService> _mockTokenService = new();
    private readonly Mock<ILogger<RefreshExpiredTokenHandler>> _mockLogger = new();
    private readonly Mock<IRoleRepository> _mockRoleRepository = new();
    private readonly RefreshExpiredTokenHandler _handler;

    public RefreshExpiredTokenHandlerTests()
    {
        _handler = new RefreshExpiredTokenHandler(_mockAuthService.Object, _mockTokenService.Object, _mockLogger.Object, _mockRoleRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidationPassed_ReturnsNewAccessToken()
    {
        // Arrange
        var email = "test@example.com";
        var accessToken = "expired_access_token";
        var refreshToken = "refresh_token";
        var userId = Guid.NewGuid();
        var userLastName = "Lastname";
        var roleName = "RoleName";

        var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(accessToken))
            .Returns(principal);

        _mockAuthService.Setup(x => x.IsRefreshTokenValid(email, refreshToken))
            .ReturnsAsync(true);

        _mockAuthService.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(new AppUser { Id = userId, LastName = userLastName, Email = email });

        _mockRoleRepository.Setup(x => x.GetAsync(userId))
            .ReturnsAsync(new Role { RoleName = roleName });

        var newAccessToken = "new_access_token";
        _mockTokenService.Setup(x => x.GenerateAccessToken(userId, userLastName, email, roleName))
            .Returns(newAccessToken);

        var command = new RefreshExpiredToken(accessToken, refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newAccessToken, result);
        _mockLogger.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"User {userId} refreshed expired token")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()!),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_InvalidAccessToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var accessToken = "expired_access_token";
        var refreshToken = "refresh_token";

        _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(accessToken))
            .Returns((ClaimsPrincipal?)null);

        var command = new RefreshExpiredToken(accessToken, refreshToken);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Cannot refresh token", ex.Message);

        _mockAuthService.Verify(x => x.IsRefreshTokenValid(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }


    [Fact]
    public async Task Handle_InvalidRefreshToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var email = "test@example.com";
        var accessToken = "expired_access_token";
        var refreshToken = "refresh_token";

        var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(accessToken))
            .Returns(principal);

        _mockAuthService.Setup(x => x.IsRefreshTokenValid(email, refreshToken))
            .ReturnsAsync(false);

        var command = new RefreshExpiredToken(accessToken, refreshToken);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockAuthService.Verify(x => x.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RoleNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var email = "test@example.com";
        var accessToken = "expired_access_token";
        var refreshToken = "refresh_token";
        var userId = Guid.NewGuid();

        var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(accessToken))
            .Returns(principal);

        _mockAuthService.Setup(x => x.IsRefreshTokenValid(email, refreshToken))
            .ReturnsAsync(true);

        _mockAuthService.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(new AppUser { Id = userId, Email = email });

        _mockRoleRepository.Setup(x => x.GetAsync(userId))
            .ReturnsAsync((Role?)null);

        var command = new RefreshExpiredToken(accessToken, refreshToken);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TokenGenerationFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var email = "test@example.com";
        var accessToken = "expired_access_token";
        var refreshToken = "refresh_token";
        var userId = Guid.NewGuid();
        var userLastName = "Lastname";
        var roleName = "RoleName";

        var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(accessToken))
            .Returns(principal);

        _mockAuthService.Setup(x => x.IsRefreshTokenValid(email, refreshToken))
            .ReturnsAsync(true);

        _mockAuthService.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(new AppUser { Id = userId, LastName = userLastName, Email = email });

        _mockRoleRepository.Setup(x => x.GetAsync(userId))
            .ReturnsAsync(new Role { RoleName = roleName });

        _mockTokenService.Setup(x => x.GenerateAccessToken(userId, userLastName, email, roleName))
            .Returns((string?)null);

        var command = new RefreshExpiredToken(accessToken, refreshToken);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
