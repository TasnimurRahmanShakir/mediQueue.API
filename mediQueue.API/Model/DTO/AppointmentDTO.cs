using mediQueue.API.Model.Entity;
using System.ComponentModel.DataAnnotations;

namespace mediQueue.API.Model.DTO
{
    public class AppointmentDTO
    {
        public class Create
        {
            [Required]
            public Guid DoctorId { get; set; }

            // Make PatientId OPTIONAL (Nullable)
            public Guid? PatientId { get; set; }

            // Add Patient Registration Fields (Optional)
            public string? Name { get; set; }
            public string? PhoneNumber { get; set; }
            public string? BloodGroup { get; set; }
            public DateTime? DOB { get; set; }

            public string? Reason { get; set; }

            [Required]
            public DateOnly AppointmentDate { get; set; }

            [Required]
            public TimeOnly AppointmentTime { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public string Reason { get; set; }
            public string Status { get; set; }
            public TimeOnly? Time { get; set; }
            public DateTime CreatedAt { get; set; }

            // Flattened Data 
            public Doctor? Doctor { get; set; }
            public Patient? Patient { get; set; }
        }

        public class AppointmentWithPatientDto
        {
            // Appointment Info
            public Guid AppointmentId { get; set; }
            public DateOnly? AppointmentDate { get; set; }
            public TimeOnly? AppointmentTime { get; set; }
            public string Status { get; set; }
            public string Reason { get; set; }

            // Nested Patient Info
            public Guid PatientId { get; set; }
            public string PatientName { get; set; }
            public string PatientPhone { get; set; }
            public string? BloodGroup { get; set; }
        }
    }
}