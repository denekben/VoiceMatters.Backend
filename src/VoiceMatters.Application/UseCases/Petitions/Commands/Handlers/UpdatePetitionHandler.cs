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

        public UpdatePetitionHandler(IPetitionRepository petitionRepository,
            ITagRepository tagRepository, ILogger<UpdatePetitionHandler> logger
            , IImageService imageService,
            IHttpContextService contextService, IAppUserRepository userRepository)
        {
            _petitionRepository = petitionRepository;
            _tagRepository = tagRepository;
            _logger = logger;
            _imageService = imageService;
            _contextService = contextService;
            _userRepository = userRepository;
        }

        public async Task<PetitionDto?> Handle(UpdatePetition command, CancellationToken cancellationToken)
        {
            var creatorId = _contextService.GetCurrentUserId();

            var creator = await _userRepository.GetAsync(creatorId)
                ?? throw new BadRequestException("Cannot find creator");

            var (id, title, textPayload, tags, images) = command;

            var petition = await _petitionRepository.GetAsync(id, PetitionIncludes.Tags | PetitionIncludes.Images)
                ?? throw new BadRequestException($"Cannot find petition {id}");

            if (petition.SignQuantity >= 1000)
                throw new BadRequestException($"Cannot update petition with 1000 and more signs");

            await _petitionRepository.DeleteAsync(petition);

            var updatedPetition = Petition.Create(id, title, textPayload, creatorId)
                ?? throw new BadRequestException($"Cannot update petition {id}");

            if (petition.CreatorId != creatorId)
                throw new BadRequestException("Authrization error");

            // adding new tags
            var tagsToCreate = tags.Except(petition.PetitionTags.Select(pt => pt.Tag.Name)).ToList();
            foreach (var tagName in tagsToCreate)
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

            // deleting tags
            var tagsToDelete = petition.PetitionTags.Select(pt => pt.Tag.Name).Except(tags).ToList();
            foreach (var tagName in tagsToDelete)
            {
                var tagToDelete = petition.PetitionTags.Select(pt => pt.Tag).First(t => t.Name == tagName);

                var petitionTagToDelete = petition.PetitionTags.First(pt => pt.TagId == tagToDelete.Id);
                petition.PetitionTags.Remove(petitionTagToDelete);
            }

            //updating images fields
            var imagesToUpdate = images.Where(i => i.Uuid != null).ToList();
            foreach (var image in imagesToUpdate)
            {
                var imageToUpdate = petition.Images.FirstOrDefault(i => i.Uuid == image.Uuid);
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
            var imagesToDelete = petition.Images.Select(i => i.Uuid).Except(images.Select(i => i.Uuid)).ToList();
            foreach (var image in imagesToCreate)
            {
                var imageURL = await _imageService.UploadFileAsync(image.File)
                    ?? throw new BadRequestException("Cannot create imageURL");
                var newImage = Image.Create(imageURL, image.Caption, image.Order, petition.Id)
                    ?? throw new BadRequestException("Cannot create new image");

                petition.Images.Add(newImage);
            }

            // deleting images
            foreach (var petitionImage in imagesToDelete)
            {
                await _imageService.DeleteByUuidAsync(petitionImage);
                var imageToDelete = petition.Images.First(i => i.Uuid == petitionImage);
                petition.Images.Remove(imageToDelete);
            }

            updatedPetition.Images = petition.Images;
            updatedPetition.PetitionTags = petition.PetitionTags;
            updatedPetition.Creator = creator;


            await _petitionRepository.AddAsync(updatedPetition);
            _logger.LogInformation($"Petition {updatedPetition.Id} updated");

            return updatedPetition.AsDto(true);
        }
    }
}
