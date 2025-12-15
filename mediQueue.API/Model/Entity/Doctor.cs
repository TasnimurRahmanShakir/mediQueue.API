using System.Text.Json.Serialization;

namespace mediQueue.API.Model.Entity
{
    public class Doctor
    {
        public Guid Id { get; set; }

        public string Specialization { get; set; } // e.g., "Cardiologist"
        public string LicenseNumber { get; set; }
        public decimal ConsultationFee { get; set; }

        // Foreign Key to User
        public Guid UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}