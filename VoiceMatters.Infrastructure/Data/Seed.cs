using VoiceMatters.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace VoiceMatters.Infrastructure.Data
{
    public sealed class Seed
    {
        public static List<Role> Roles { get; private set; } = [];
        public static Statistic Statistic;

        static Seed()
        {
            Roles = [
                Role.Admin,
                Role.User
            ];

            Statistic = Statistic.Create();
        }
    }
}
