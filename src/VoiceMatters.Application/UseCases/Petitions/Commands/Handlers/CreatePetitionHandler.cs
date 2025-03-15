using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Application.UseCases.Petitions.Commands.Handlers
{
    public sealed class CreatePetitionHandler : IRequestHandler<CreatePetition, PetitionDto?>
    {
        private readonly IPetitionRepository _petitionRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IImageService _imageService;
        private readonly ILogger<CreatePetitionHandler> _logger;
        private readonly IHttpContextService _contextService;
        private readonly IStatisticRepository _statisticRepository;
        private readonly INotificationService _notifications;
        private readonly IAppUserRepository _userRepository;

        public CreatePetitionHandler(IPetitionRepository petitionRepository,
            ITagRepository tagRepository, ILogger<CreatePetitionHandler> logger
            , IImageService imageService, IHttpContextService contextService, IStatisticRepository statisticRepository,
            INotificationService notifications, IAppUserRepository userRepository)
        {
            _petitionRepository = petitionRepository;
            _tagRepository = tagRepository;
            _logger = logger;
            _imageService = imageService;
            _contextService = contextService;
            _statisticRepository = statisticRepository;
            _notifications = notifications;
            _userRepository = userRepository;
        }

        public async Task<PetitionDto?> Handle(CreatePetition command, CancellationToken cancellationToken)
        {
            var creatorId = _contextService.GetCurrentUserId();

            var creator = await _userRepository.GetAsync(creatorId)
                ?? throw new BadRequestException("Cannot find creator");

            var (title, textPayload, tags, images) = command;

            var petition = Petition.Create(title, textPayload, creatorId)
                ?? throw new BadRequestException("Cannot create petition");

            foreach (var tagName in tags)
            {
                var tag = await _tagRepository.GetTagByNameAsync(tagName);
                if (tag == null)
                {
                    tag = Tag.Create(tagName)
                        ?? throw new BadRequestException($"Cannot create tag {tagName}");
                }

                var petitionTag = PetitionTag.Create(petition.Id, tag.Id)
                    ?? throw new BadRequestException("Cannot create PetitionTag");
                petitionTag.Tag = tag;
                petition.PetitionTags.Add(petitionTag);
            }

            foreach (var image in images)
            {
                var imageURL = await _imageService.UploadFileAsync(image.File)
                    ?? throw new BadRequestException("Cannot create imageURL");
                var newImage = Image.Create(imageURL, image.Caption, image.Order, petition.Id)
                    ?? throw new BadRequestException("Cannot create new image");

                petition.Images.Add(newImage);
            }

            var sign = AppUserSignedPetition.Create(creatorId, petition.Id)
                ?? throw new BadRequestException("Cannot sign petition");
            petition.SignedUsers.Add(sign);

            petition.Creator = creator;

            await _petitionRepository.AddAsync(petition);
            _logger.LogInformation($"Petition {petition.Id} created");

            var stats = await _statisticRepository.GetAsync();
            if (stats != null)
            {
                stats.Update(StatParameter.PetitionQuantity);
                stats.Update(StatParameter.SignsQuantity);
                await _statisticRepository.UpdateAsync(stats);
                await _notifications.PetitionCreated();
                await _notifications.PetitionSigned();
            }

            return petition.AsDto(true);
        }
    }
}
