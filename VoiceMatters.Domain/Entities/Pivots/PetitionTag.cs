using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Domain.Entities.Pivots
{
    public sealed class PetitionTag
    {
        public Guid PetitionId { get; private set; }
        public Guid TagId { get; private set; }
        public Petition Petition { get; private set; }
        public Tag Tag { get; private set; }

        private PetitionTag() {}

        private PetitionTag(Guid petitionId, Guid tagId)
        {
            PetitionId = petitionId;
            TagId = tagId;
        }

        public static PetitionTag Create(Guid petitionId, Guid tagId)
        {
            return new(petitionId, tagId);
        }
    }
}
