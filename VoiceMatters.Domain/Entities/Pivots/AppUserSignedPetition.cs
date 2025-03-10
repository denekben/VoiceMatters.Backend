namespace VoiceMatters.Domain.Entities.Pivots
{
    public sealed class AppUserSignedPetition
    {
        public Guid SignerId { get; private set; }
        public Guid PetitionId { get; private set; }
        public DateTime SignedDate { get; private set; }
        public AppUser Signer { get; private set; }
        public Petition Petition { get; private set; }

        private AppUserSignedPetition(Guid signerId, Guid petitionId) 
        {
            SignerId = signerId;
            PetitionId = petitionId;
            SignedDate = DateTime.UtcNow;
        }

        public static AppUserSignedPetition Create(Guid signerId, Guid petitionId)
        {
            return new AppUserSignedPetition(signerId, petitionId);
        }
    }
}
