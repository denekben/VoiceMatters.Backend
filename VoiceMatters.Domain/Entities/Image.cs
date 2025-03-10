using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Domain.Entities
{
    public sealed class Image
    {
        private const int _maxCaptionLength = 50;

        public Guid Id { get; private set; }
        public string Uuid { get; private set; }
        public string Caption { get; private set; }
        public uint Order { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; set; }

        public Guid PetitionId { get; private set; }
        public Petition Petition { get; private set; }

        private Image()
        {
            
        }

        private Image(string imageUuid, string? caption, uint order, Guid petitionId)
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
            if(caption != null && (caption.Length > _maxCaptionLength || string.IsNullOrWhiteSpace(caption)))
                throw new InvalidArgumentDomainException($"Invalid argument for Image[caption]. Entered value: {caption}");

            return new Image(imageUuid, caption, order, petitionId);
        }
    }
}
