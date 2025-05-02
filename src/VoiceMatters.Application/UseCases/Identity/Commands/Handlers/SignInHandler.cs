using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Identity.Commands.Handlers
{
    public sealed class SignInHandler : IRequestHandler<SignIn, TokensDto?>
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<SignInHandler> _logger;
        private readonly IRepository _repository;

        public SignInHandler(IAuthService authService, ITokenService tokenService, ILogger<SignInHandler> logger,
            IRepository repository)
        {
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
            _repository = repository;
        }

        public async Task<TokensDto?> Handle(SignIn command, CancellationToken cancellationToken)
        {
            var (email, password) = command;

            await _authService.SigninUserAsync(email, password);
            var user = await _authService.GetUserByEmailAsync(email);

            var refreshToken = _tokenService.GenerateRefreshToken();

            await _authService.UpdateRefreshToken(email, refreshToken);

            string accessToken = _tokenService.GenerateAccessToken(user.Id, user.LastName, email, user.Role.RoleName)
                ?? throw new InvalidOperationException("Cannot create access token");

            await _repository.SaveChangesAsync();
            _logger.LogInformation($"User {email} signed in");
            return new TokensDto(accessToken, refreshToken);
        }
    }
}
