using System.Text.RegularExpressions;
using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Domain.Entities
{
    public sealed class AppUser
    {
        private const int _minNameLength = 2;
        private const int _maxNameLength = 30;

        private static readonly Regex _phonePattern = new(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$", RegexOptions.IgnoreCase);
        private static readonly Regex _emailPattern = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase);
        private static readonly List<string> _allowedSex = ["Male", "Female"];

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Sex { get; set; }
        public string? ImageUuid { get; set; }
        public bool IsBlocked { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public List<Petition> PetitionsCreatedByUser { get; set; } = [];
        public List<AppUserSignedPetition> PetitionsSignedByUser { get; set; } = [];

        public AppUser()
        {

        }

        public AppUser(string firstName, string lastName, string? phone,
            string email, string passwordHash, DateTime? birthDate, string? sex,
            string? imageURL, Guid roleId)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Email = email;
            PasswordHash = passwordHash;
            BirthDate = birthDate;
            Sex = sex;
            ImageUuid = imageURL;
            RoleId = roleId;
            CreatedDate = DateTime.UtcNow;
        }

        public static AppUser Create(string firstName, string lastName, string? phone,
            string email, string password, DateTime? birthDate, string? sex,
            string? imageURL, Guid roleId)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < _minNameLength ||
                firstName.Length > _maxNameLength || firstName.Contains(' '))
                throw new InvalidArgumentDomainException($"Invalid argument for AppUser[firstName]. Entered value: {firstName}");

            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < _minNameLength ||
                lastName.Length > _maxNameLength || lastName.Contains(' '))
                throw new InvalidArgumentDomainException($"Invalid argument for AppUser[lastName]. Entered value: {lastName}");

            if (phone != null && !_phonePattern.IsMatch(phone))
                throw new InvalidArgumentDomainException($"Invalid argument for AppUser[phone]. Entered value: {phone}");

            if (string.IsNullOrWhiteSpace(email) || !_emailPattern.IsMatch(email))
                throw new InvalidArgumentDomainException($"Invalid argument for AppUser[email]. Entered value: {email}");

            if (birthDate != null && birthDate >= DateTime.UtcNow)
                throw new InvalidArgumentDomainException($"Invalid argument for AppUser[birthDate]. Entered value: {birthDate}");

            if (sex != null && (sex != _allowedSex[0] && sex != _allowedSex[1]))
                throw new InvalidArgumentDomainException($"Invalid argument for AppUser[sex]. Entered value: {sex}");

            return new AppUser(firstName, lastName, phone, email,
                password, birthDate, sex, imageURL, roleId);
        }
    }
}
