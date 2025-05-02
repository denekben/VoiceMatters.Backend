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
    public sealed class UpdatePetitionHandler : IRequestHandler<UpdatePetition, PetitionDto?>
    {
        private readonly IPetitionRepository _petitionRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IImageService _imageService;
        private readonly ILogger<UpdatePetitionHandler> _logger;
        private readonly IHttpContextService _contextService;
        private readonly IAppUserRepository _userRepository;
        private readonly IPetitionTagRepository _petitionTagRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IRepository _repository;

        public UpdatePetitionHandler(IPetitionRepository petitionRepository,
            ITagRepository tagRepository, ILogger<UpdatePetitionHandler> logger
            , IImageService imageService,
            IHttpContextService contextService, IAppUserRepository userRepository,
            IPetitionTagRepository petitionTagRepository, IRepository repository, IImageRepository imageRepository)
        {
            _petitionRepository = petitionRepository;
            _tagRepository = tagRepository;
            _logger = logger;
            _imageService = imageService;
            _contextService = contextService;
            _userRepository = userRepository;
            _petitionTagRepository = petitionTagRepository;
            _repository = repository;
            _imageRepository = imageRepository;
        }

        public async Task<PetitionDto?> Handle(UpdatePetition command, CancellationToken cancellationToken)
        {
            var creatorId = _contextService.GetCurrentUserId();

            var creator = await _userRepository.GetAsync(creatorId)
                ?? throw new BadRequestException("Cannot find creator");

            var (id, title, textPayload, tagNames, images) = command;

            var petition = await _petitionRepository.GetAsync(id)
                ?? throw new BadRequestException($"Cannot find petition {id}");
            var loadedImages = await _imageRepository.GetByPetitionIdAsync(id) ?? [];

            if (petition.SignQuantity >= 1000)
                throw new BadRequestException($"Cannot update petition with 1000 and more signs");

            if (petition.CreatorId != creatorId)
                throw new BadRequestException("Authrization error");

            petition.UpdateDetails(title, textPayload, creatorId);

            await _petitionTagRepository.DeleteByPetitionIdAsync(id);

            var inputTag = new List<Tag>();
            foreach (var tagName in tagNames)
            {
                var tag = await _tagRepository.GetByNameNoTrackingAsync(tagName);
                if (tag == null)
                {
                    tag = Tag.Create(tagName);
                    await _tagRepository.AddAsync(tag);
                }
                inputTag.Add(tag);
                var petitionTag = PetitionTag.Create(id, tag.Id);
                await _petitionTagRepository.AddAsync(petitionTag);
            }

            //updating images fields
            var imagesToUpdate = images.Where(i => i.Uuid != null).ToList();
            foreach (var image in imagesToUpdate)
            {
                var imageToUpdate = loadedImages.FirstOrDefault(i => i.Uuid == image.Uuid);
                if (imageToUpdate != null)
                {
                    if (image.Uuid != null)
                        imageToUpdate.Uuid = image.Uuid;
                    imageToUpdate.Caption = image.Caption;
                    imageToUpdate.Order = image.Order;
                    imageToUpdate.PetitionId = petition.Id;
                }
            }

            // adding new images
            var imagesToCreate = images.Where(i => i.Uuid == null).ToList();
            var imagesToDelete = loadedImages.Select(i => i.Uuid).Except(images.Select(i => i.Uuid)).ToList();
            foreach (var image in imagesToCreate)
            {
                var imageURL = await _imageService.UploadFileAsync(image.File)
                    ?? throw new BadRequestException("Cannot create imageURL");
                var newImage = Image.Create(imageURL, image.Caption, image.Order, petition.Id)
                    ?? throw new BadRequestException("Cannot create new image");

                loadedImages.Add(newImage);
            }

            // deleting images
            foreach (var petitionImage in imagesToDelete)
            {
                await _imageService.DeleteByUuidAsync(petitionImage);
                var imageToDelete = loadedImages.First(i => i.Uuid == petitionImage);
                loadedImages.Remove(imageToDelete);
            }

            await _repository.SaveChangesAsync();
            _logger.LogInformation($"Petition {petition.Id} updated");

            return petition.AsDto(true, inputTag, loadedImages);
        }
    }
}
