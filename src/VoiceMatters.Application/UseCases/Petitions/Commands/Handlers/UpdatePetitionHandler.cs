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

            var petition = await _petitionRepository.GetAsync(id,
                PetitionIncludes.Tags | PetitionIncludes.PetitionTags | PetitionIncludes.Images)
                ?? throw new BadRequestException($"Cannot find petition {id}");

            var updatedPetition = Petition.Create(id, title, textPayload, creatorId)
                ?? throw new BadRequestException($"Cannot update petition {id}");

            if (petition.CreatorId != creatorId)
                throw new BadRequestException("Authrization error");

            // adding new tags
            var tagsToCreate = tags.Except(petition.Tags.Select(t => t.Name));
            foreach (var tagName in tagsToCreate)
            {
                var tag = await _tagRepository.GetTagByNameAsync(tagName);
                if (tag == null)
                {
                    tag = Tag.Create(tagName)
                        ?? throw new BadRequestException($"Cannot create tag {tagName}");
                }
                petition.Tags.Add(tag);

                var petitionTag = PetitionTag.Create(petition.Id, tag.Id)
                    ?? throw new BadRequestException("Cannot create PetitionTag");
                petition.PetitionTags.Add(petitionTag);
            }

            // deleting tags
            var tagsToDelete = petition.Tags.Select(t => t.Name).Except(tags);
            foreach (var tagName in tagsToDelete)
            {
                var tagToDelete = petition.Tags.First(t => t.Name == tagName);
                petition.Tags.Remove(tagToDelete);

                var petitionTagToDelete = petition.PetitionTags.First(pt => pt.TagId == tagToDelete.Id);
                petition.PetitionTags.Remove(petitionTagToDelete);
            }

            // adding new images
            var imagesToCreate = images.Where(i => i.Uuid == null);
            foreach (var image in imagesToCreate)
            {
                var imageURL = await _imageService.UploadFileAsync(image.File)
                    ?? throw new BadRequestException("Cannot create imageURL");
                var newImage = Domain.Entities.Image.Create(imageURL, image.Caption, image.Order, petition.Id)
                    ?? throw new BadRequestException("Cannot create new image");

                petition.Images.Add(newImage);
            }

            // deleting images
            var imagesToDelete = petition.Images.Select(i => i.Uuid).Except(images.Select(i => i.Uuid));
            foreach (var petitionImage in imagesToDelete)
            {
                await _imageService.DeleteByUuidAsync(petitionImage);
                var imageToDelete = petition.Images.First(i => i.Uuid == petitionImage);
                petition.Images.Remove(imageToDelete);
            }

            updatedPetition.Images = petition.Images;
            updatedPetition.PetitionTags = petition.PetitionTags;
            updatedPetition.Tags = petition.Tags;
            updatedPetition.Creator = creator;

            await _petitionRepository.UpdateAsync(updatedPetition);
            _logger.LogInformation($"Petition {updatedPetition.Id} updated");

            return updatedPetition.AsDto(true);
        }
    }
}
