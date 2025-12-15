using System.Numerics;
using System.Text.Json.Serialization;

namespace mediQueue.API.Model.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string ImageUrl { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Doctor? DoctorProfile { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Receptionist? ReceptionistProfile { get; set; }
    }
}