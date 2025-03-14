namespace VoiceMatters.Domain.Entities.Pivots
{
    public sealed class PetitionTag
    {
        public Guid Id { get; private set; }
        public Guid PetitionId { get; set; }
        public Guid TagId { get; set; }
        public Petition Petition { get; set; }
        public Tag Tag { get; set; }

        public PetitionTag()
        {

        }

        public PetitionTag(Guid petitionId, Guid tagId)
        {
            Id = Guid.NewGuid();
            PetitionId = petitionId;
            TagId = tagId;
        }

        public static PetitionTag Create(Guid petitionId, Guid tagId)
        {
            return new(petitionId, tagId);
        }
    }
}
