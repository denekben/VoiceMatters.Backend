using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Domain.Entities
{
    public sealed class Tag
    {
        private const int _minTagName = 2;
        private const int _maxTagName = 25;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<PetitionTag> Petitions { get; set; } = [];

        public Tag() { }

        public Tag(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            CreatedDate = DateTime.UtcNow;
        }

        public static Tag Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < _minTagName || name.Length > _maxTagName)
                throw new InvalidArgumentDomainException($"Invalid argument for Tag[name]. Entered value: {name}");
            return new(name);
        }
    }
}
