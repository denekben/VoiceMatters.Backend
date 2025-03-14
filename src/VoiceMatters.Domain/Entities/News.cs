using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Domain.Entities
{
    public sealed class News
    {
        private const int _minTitleLength = 10;
        private const int _maxTitleLength = 200;

        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Guid PetitionId { get; set; }
        public Petition Petition { get; set; }

        public News()
        {

        }

        public News(string title, Guid petitionId)
        {
            Id = Guid.NewGuid();
            Title = title;
            CreatedDate = DateTime.UtcNow;
            PetitionId = petitionId;
        }

        public News(Guid id, string title, Guid petitionId)
        {
            Id = id;
            Title = title;
            CreatedDate = DateTime.UtcNow;
            PetitionId = petitionId;
        }

        public static News Create(string title, Guid petitionId)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Length < _minTitleLength || title.Length > _maxTitleLength)
                throw new InvalidArgumentDomainException($"Invalid argument for News[title]. Entered value: {title}");

            return new(title, petitionId);
        }

        public static News Create(Guid id, string title, Guid petitionId)
        {
            if (id == Guid.Empty)
                throw new InvalidArgumentDomainException($"Invalid argument for News[id]. Entered value: {id}");
            if (string.IsNullOrWhiteSpace(title) || title.Length < _minTitleLength || title.Length > _maxTitleLength)
                throw new InvalidArgumentDomainException($"Invalid argument for News[title]. Entered value: {title}");

            return new(id, title, petitionId);
        }
    }
}