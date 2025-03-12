using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Domain.Entities
{
    public sealed class Image
    {
        private const int _maxCaptionLength = 50;

        public Guid Id { get; set; }
        public string Uuid { get; set; }
        public string? Caption { get; set; }
        public uint Order { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Guid PetitionId { get; set; }
        public Petition Petition { get; set; }

        public Image()
        {

        }

        public Image(string imageUuid, string? caption, uint order, Guid petitionId)
        {
            Id = Guid.NewGuid();
            Uuid = imageUuid;
            Caption = caption ?? string.Empty;
            Order = order;
            PetitionId = petitionId;
            CreatedDate = DateTime.UtcNow;
        }

        public static Image Create(string imageUuid, string? caption, uint order, Guid petitionId)
        {
            if (string.IsNullOrWhiteSpace(imageUuid))
                throw new InvalidArgumentDomainException($"Invalid argument for Image[imageUuid]. Entered value: {imageUuid}");
            if (caption?.Length > _maxCaptionLength)
                throw new InvalidArgumentDomainException($"Invalid argument for Image[caption]. Entered value: {caption}");

            return new Image(imageUuid, caption, order, petitionId);
        }
    }
}
