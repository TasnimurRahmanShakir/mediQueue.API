using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace mediQueue.API.Model.Entity
{
    public class Doctor
    {
        public Guid Id { get; set; }

        public string Specialization { get; set; } // e.g., "Cardiologist"
        public string LicenseNumber { get; set; }
        public decimal ConsultationFee { get; set; }

        [Required]
        public TimeOnly CounsilingStart { get; set; }
        [Required]
        public TimeOnly CounsilingEnd { get; set; }

        // Foreign Key to User
        public Guid UserId { get; set; }
        public User User { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}