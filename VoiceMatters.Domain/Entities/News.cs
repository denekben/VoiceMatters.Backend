using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Domain.Entities
{
    public sealed class News
    {
        private const int _minTitleLength = 10;
        private const int _maxTitleLength = 200;

        public Guid Id { get; private set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; set; }

        public Guid PetitionId { get; private set; }
        public Petition Petition { get; set; }

        private News()
        {
            
        }

        private News(string title, Guid petitionId)
        {
            Id = Guid.NewGuid();
            Title = title;
            CreatedDate = DateTime.UtcNow;
            PetitionId = petitionId;
        }

        private News(Guid id, string title, Guid petitionId)
        {
            Title = title;
            CreatedDate = DateTime.UtcNow;
            PetitionId = petitionId;
        }

        public static News Create(string title, Guid petitionId)
        {
            if(string.IsNullOrWhiteSpace(title) || title.Length < _minTitleLength || title.Length > _maxTitleLength)
                throw new InvalidArgumentDomainException($"Invalid argument for News[title]. Entered value: {title}");

            return new(title, petitionId);
        }

        public static News Create(Guid id, string title, Guid petitionId)
        {
            if(id == Guid.Empty)
                throw new InvalidArgumentDomainException($"Invalid argument for News[id]. Entered value: {id}");
            if (string.IsNullOrWhiteSpace(title) || title.Length < _minTitleLength || title.Length > _maxTitleLength)
                throw new InvalidArgumentDomainException($"Invalid argument for News[title]. Entered value: {title}");

            return new(id, title, petitionId);
        }
    }
}