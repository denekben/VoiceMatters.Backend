using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Infrastructure.Data
{
    public sealed class Seed
    {
        public static List<Role> Roles { get; set; } = [];
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
