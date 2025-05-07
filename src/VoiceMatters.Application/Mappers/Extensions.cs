using VoiceMatters.Domain.Entities;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.Mappers
{
    public static class Extensions
    {
        public static PetitionDto AsDto(this Petition petition, bool signedByCurrentUser)
        {
            var tags = petition.PetitionTags.Select(pt => pt.Tag.AsDto()).ToList();
            var images = petition.Images.Select(i => i.AsDto()).ToList();

            return new(
                petition.Id,
                petition.Title,
                petition.TextPayload,
                petition.SignQuantity,
                petition.SignQuantityPerDay,
                petition.IsCompleted,
                petition.IsBlocked,
                signedByCurrentUser,
                tags,
                images,
                petition.News?.Id,
                petition.News?.Title,
                petition.Creator.AsProfilePlateDto(),
                petition.CreatedDate,
                petition.UpdatedDate
            );
        }

        public static PetitionDto AsDto(this Petition petition, bool signedByCurrentUser, List<Tag> inputTags, List<Image> inputImages)
        {
            var tags = (inputTags ?? []).Select(t => t.AsDto()).ToList();
            var images = (inputImages ?? []).Select(i => i.AsDto()).ToList();

            return new(
                petition.Id,
                petition.Title,
                petition.TextPayload,
                petition.SignQuantity,
                petition.SignQuantityPerDay,
                petition.IsCompleted,
                petition.IsBlocked,
                signedByCurrentUser,
                tags,
                images,
                petition.News?.Id,
                petition.News?.Title,
                petition.Creator.AsProfilePlateDto(),
                petition.CreatedDate,
                petition.UpdatedDate
            );
        }

        public static ImageDto AsDto(this Image image)
        {
            return new(image.Id, image.Uuid, image.Caption, image.Order);
        }

        public static TagDto AsDto(this Tag tag)
        {
            return new(tag.Id, tag.Name);
        }

        public static NewsDto AsDto(this News news)
        {
            var petition = news.Petition;
            var tags = petition.PetitionTags.Select(pt => pt.Tag.AsDto()).ToList();
            var images = petition.Images.Select(i => i.AsDto()).ToList();

            return new(
                news.Id,
                news.Title,
                news.PetitionId,
                images,
                tags,
                petition.SignQuantity,
                petition.IsBlocked
            );
        }

        public static ProfileDto AsProfileDto(this AppUser user)
        {

            return new(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Phone,
                user.Email,
                user.BirthDate,
                user.Sex,
                user.ImageUuid,
                user.IsBlocked,
                user.PetitionsCreatedByUser.Count,
                user.PetitionsSignedByUser.Count,
                user.CreatedDate
            );
        }

        public static ProfilePlateDto AsProfilePlateDto(this AppUser user)
        {
            return new(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Sex,
                user.ImageUuid,
                user.IsBlocked
            );
        }
    }
}