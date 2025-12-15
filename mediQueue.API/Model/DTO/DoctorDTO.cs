using System.ComponentModel.DataAnnotations;

namespace mediQueue.API.Model.DTO
{
    public class DoctorDTO
    {
        public class Create
        {
            [Required]
            public Guid UserId { get; set; }

            [Required]
            public string Specialization { get; set; }

            [Required]
            public string LicenseNumber { get; set; }

            [Required]
            [Range(0, 5000)]
            public decimal ConsultationFee { get; set; }
        }

        public class Update
        {
            [Required]
            public Guid Id { get; set; }
            public string Specialization { get; set; }
            public decimal ConsultationFee { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public string Specialization { get; set; }
            public string LicenseNumber { get; set; }
            public decimal ConsultationFee { get; set; }


        }
    }
}