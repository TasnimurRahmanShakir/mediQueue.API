using System.ComponentModel.DataAnnotations;

namespace mediQueue.API.Model.DTO
{
    public class AppointmentDTO
    {
        public class Create
        {
            [Required]
            public Guid DoctorId { get; set; }

            [Required]
            public Guid PatientId { get; set; }

            [Required]
            public string Reason { get; set; }

            [Required]
            public DateTime Schedule { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public string Reason { get; set; }
            public string Status { get; set; }
            public DateTime? Schedule { get; set; }
            public DateTime CreatedAt { get; set; }

            // Flattened Data 
            public Guid DoctorId { get; set; }
            public string DoctorName { get; set; }

            public Guid PatientId { get; set; }
            public string PatientName { get; set; }
        }
    }
}