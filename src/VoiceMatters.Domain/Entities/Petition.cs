using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Domain.Entities
{
    public sealed class Petition
    {
        private const int _minTitleLength = 10;
        private const int _maxTitleLength = 200;
        private const int _minTextPayloadLength = 200;
        private const int _maxTextPayloadLength = 2000;

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string TextPayload { get; set; }
        public uint SignQuantity { get; set; }
        public uint SignQuantityPerDay { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Guid? CreatorId { get; set; }
        public AppUser Creator { get; set; }
        public List<PetitionTag> PetitionTags { get; set; } = [];
        public List<Image> Images { get; set; } = [];
        public List<AppUserSignedPetition> SignedUsers { get; set; } = [];
        public News? News { get; set; }

        public Petition()
        {

        }

        public Petition(string title, string textPayload, Guid creatorId)
        {
            Id = Guid.NewGuid();
            Title = title;
            TextPayload = textPayload;
            SignQuantity = 1;
            SignQuantityPerDay = 1;

            CreatedDate = DateTime.UtcNow;
            CreatorId = creatorId;
        }

        public Petition(Guid id, string title, string textPayload, Guid creatorId)
        {
            Id = id;
            Title = title;
            TextPayload = textPayload;
            SignQuantity = 0;
            SignQuantityPerDay = 0;

            CreatedDate = DateTime.UtcNow;
            CreatorId = creatorId;
        }

        public static Petition Create(string title, string textPayload, Guid creatorId)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Length < _minTitleLength || title.Length > _maxTitleLength)
                throw new InvalidArgumentDomainException($"Invalid argument for Petition[title]. Entered value: {title}");
            if (string.IsNullOrWhiteSpace(textPayload) || textPayload.Length < _minTextPayloadLength || textPayload.Length > _maxTextPayloadLength)
                throw new InvalidArgumentDomainException($"Invalid argument for Petition[title]. Entered value: {textPayload}");
            return new(title, textPayload, creatorId);
        }

        public static Petition Create(Guid id, string title, string textPayload, Guid creatorId)
        {
            if (id == Guid.Empty)
                throw new InvalidArgumentDomainException($"Invalid argument for Petition[id]. Entered value: {id}");
            if (string.IsNullOrWhiteSpace(title) || title.Length < _minTitleLength || title.Length > _maxTitleLength)
                throw new InvalidArgumentDomainException($"Invalid argument for Petition[title]. Entered value: {title}");
            if (string.IsNullOrWhiteSpace(textPayload) || textPayload.Length < _minTextPayloadLength || textPayload.Length > _maxTextPayloadLength)
                throw new InvalidArgumentDomainException($"Invalid argument for Petition[textPayload]. Entered value: {textPayload}");
            return new(id, title, textPayload, creatorId);
        }
    }
}
