using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Domain.Entities
{
    public sealed class Role
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; set; }

        public List<AppUser>? Users { get; set; } = [];

        public static Role Admin => new Role(nameof(Admin));
        public static Role User => new Role(nameof(User));

        private Role() { }

        private Role(string roleName)
        {
            Id = Guid.NewGuid();
            RoleName = roleName;
            CreatedDate = DateTime.UtcNow;
        }

        public static Role Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new InvalidArgumentDomainException($"Invalid argument for Role[roleName]. Entered value: {roleName}");
            return new(roleName);
        }
    }
}
