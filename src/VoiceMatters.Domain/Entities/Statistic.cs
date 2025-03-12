namespace VoiceMatters.Domain.Entities
{
    public sealed class Statistic
    {
        public Guid Id { get; set; }
        public int UserQuantity { get; set; }
        public int PetitionQuantity { get; set; }
        public int SignsQuantity { get; set; }

        public Statistic() { }

        public Statistic(int userQuantity, int petitionQuantity, int signsQuantity)
        {
            Id = Guid.NewGuid();
            UserQuantity = userQuantity;
            PetitionQuantity = petitionQuantity;
            SignsQuantity = signsQuantity;
        }

        public static Statistic Create()
        {
            return new(0, 0, 0);
        }

        public void Update(StatParameter parameter, int deltaValue = 1)
        {
            if (parameter == StatParameter.UserQuantity)
                UserQuantity += deltaValue;
            else if (parameter == StatParameter.PetitionQuantity)
                PetitionQuantity += deltaValue;
            else if (parameter == StatParameter.SignsQuantity)
                SignsQuantity += deltaValue;
        }
    }

    public enum StatParameter
    {
        UserQuantity,
        PetitionQuantity,
        SignsQuantity
    }
}
