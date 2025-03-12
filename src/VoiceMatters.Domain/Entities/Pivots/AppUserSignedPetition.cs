namespace VoiceMatters.Domain.Entities.Pivots
{
    public sealed class AppUserSignedPetition
    {
        public Guid SignerId { get; set; }
        public Guid PetitionId { get; set; }
        public DateTime SignedDate { get; set; }
        public AppUser Signer { get; set; }
        public Petition Petition { get; set; }

        public AppUserSignedPetition()
        {

        }

        public AppUserSignedPetition(Guid signerId, Guid petitionId)
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
