﻿using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Identity.Commands.Handlers
{
    internal sealed class RegisterNewUserHandler : IRequestHandler<RegisterNewUser, TokensDto?>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<RegisterNewUserHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IRoleRepository _roleRepository;
        private readonly IImageService _imageService;
        private readonly IStatisticRepository _statisticRepository;
        private readonly INotificationService _notifications;

        public RegisterNewUserHandler(IAuthService authService, ILogger<RegisterNewUserHandler> logger, ITokenService tokenService,
            IRoleRepository roleRepository, IImageService imageService, IStatisticRepository statisticRepository
            , INotificationService notifications)
        {
            _authService = authService;
            _logger = logger;
            _tokenService = tokenService;
            _roleRepository = roleRepository;
            _imageService = imageService;
            _statisticRepository = statisticRepository;
            _notifications = notifications;
        }

        public async Task<TokensDto?> Handle(RegisterNewUser command, CancellationToken cancellationToken)
        {
            var (firstName, lastName, phone, password, email, dateOfBirth, sex, image) = command;

            var role = await _roleRepository.GetByNameAsync(Role.User.RoleName)
                ?? throw new BadRequestException($"Cannot find role with name {Role.User.RoleName}");

            var hashedPassword = _authService.HashPassword(password);

            var imageURL = await _imageService.UploadFileAsync(image);

            var user = await _authService.CreateUserAsync(firstName, lastName, phone, email, hashedPassword, dateOfBirth, sex, imageURL, role.Id);

            var refreshToken = _tokenService.GenerateRefreshToken();

            await _authService.UpdateRefreshToken(email, refreshToken);
            var accessToken = _tokenService.GenerateAccessToken(user.Id, lastName, email, Role.User.RoleName)
                ?? throw new InvalidOperationException("Cannot generate access token");

            var stats = await _statisticRepository.GetAsync();
            if(stats != null)
            {
                stats.Update(StatParameter.UserQuantity);
                await _statisticRepository.UpdateAsync(stats);
                await _notifications.UserRegistered();
            }

            _logger.LogInformation($"User {email} registered");
            return new TokensDto(accessToken, refreshToken);
        }
    }
}
