using mediQueue.API.Model.Entity;
using System.ComponentModel.DataAnnotations;

namespace mediQueue.API.Model.DTO
{
    public class UserDTO
    {
        //  REGISTER
        public class Create
        {
            // --- Existing User Fields ---
            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Phone]
            public string PhoneNumber { get; set; }

            [Required]
            [MinLength(6)]
            public string Password { get; set; }

            public IFormFile? Image { get; set; }
            public string? ImageUrl { get; set; }

            [Required]
            public required string Role { get; set; } // e.g., "Doctor", "Patient", "Receptionist"

            // --- NEW: Doctor Specific Fields (Nullable) ---
            public string? Specialization { get; set; }
            public string? LicenseNumber { get; set; }
            public decimal? ConsultationFee { get; set; }

            [Required]
            public TimeOnly CounsilingStart { get; set; }
            [Required]
            public TimeOnly CounsilingEnd { get; set; }

            public string? ShiftTime { get; set; }
        }

        public class Edit
        {

            
            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [Required]
            [Phone]
            public required string PhoneNumber { get; set; }
            public IFormFile? Image { get; set; }
            public string? ImageUrl { get; set; }

            public string? Status { get; set; }

            public decimal? ConsultationFee { get; set; }
           
            public TimeOnly CounsilingStart { get; set; }
            
            public TimeOnly CounsilingEnd { get; set; }

            public string? ShiftTime { get; set; }
        }

        // LOGIN REQUEST
        public class LoginRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
        }

        //  RESPONSE 
        public class Response
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string ImageUrl { get; set; }
            public string Role { get; set; }
            public string Status { get; set; }
            public DoctorDTO.Response? DoctorProfile { get; set; }
            public ReceptionistDTO.Response? ReceptionistProfile { get; set; }

        }


        public class RefreshRequest { public string RefreshToken { get; set; } }

    }
}