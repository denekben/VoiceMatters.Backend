using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Application.UseCases.Petitions.Commands.Handlers
{
    public sealed class DeletePetitionHandler : IRequestHandler<DeletePetition>
    {
        private readonly IPetitionRepository _petitionRepository;
        private readonly IImageService _imageService;
        private readonly ILogger<DeletePetitionHandler> _logger;
        private readonly IHttpContextService _contextService;
        private readonly IStatisticRepository _statisticRepository;
        private readonly INotificationService _notifications;

        public DeletePetitionHandler(IPetitionRepository petitionRepository, ILogger<DeletePetitionHandler> logger,
            IImageService imageService, IHttpContextService contextService, IStatisticRepository statisticRepository,
            INotificationService notifications)
        {
            _petitionRepository = petitionRepository;
            _logger = logger;
            _imageService = imageService;
            _contextService = contextService;
            _statisticRepository = statisticRepository;
            _notifications = notifications;
        }

        public async Task Handle(DeletePetition command, CancellationToken cancellationToken)
        {
            var creatorId = _contextService.GetCurrentUserId();

            var petition = await _petitionRepository.GetAsync(command.Id, PetitionIncludes.Images)
                ?? throw new BadRequestException($"Cannot find petition {command.Id}");

            if (petition.CreatorId != creatorId)
                throw new BadRequestException("Authrization error");

            var stats = await _statisticRepository.GetAsync();
            if (stats != null)
            {
                stats.Update(StatParameter.PetitionQuantity, -1);
                stats.Update(StatParameter.SignsQuantity, -1 * (int)petition.SignQuantity);
                await _statisticRepository.UpdateAsync(stats);
                await _notifications.PetitionDeleted((int)petition.SignQuantity);
            }

            // deleting images
            foreach (var images in petition.Images)
            {
                await _imageService.DeleteByUuidAsync(images.Uuid);
            }

            await _petitionRepository.DeleteAsync(petition);
            _logger.LogInformation($"Petition {petition.Id} deleted");
        }
    }
}
