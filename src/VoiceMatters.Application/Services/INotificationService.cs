namespace VoiceMatters.Application.Services
{
    public interface INotificationService
    {
        Task PetitionSigned();
        Task PetitionCreated();
        Task PetitionDeleted(int signsQuantity);
        Task UserRegistered();
    }
}
