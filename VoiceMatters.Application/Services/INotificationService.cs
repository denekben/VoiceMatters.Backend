using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.Services
{
    public interface INotificationService
    {
        Task PetitionSigned();
        Task PetitionCreated();
        Task PetitionDeleted();
        Task UserRegistered();
    }
}
