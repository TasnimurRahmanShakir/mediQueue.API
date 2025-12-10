using System.Numerics;

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
        public Doctor? DoctorProfile { get; set; }
        public Receptionist? ReceptionistProfile { get; set; }
    }
}