using mediQueue.API.Model.Entity;
using System.ComponentModel.DataAnnotations;

namespace mediQueue.API.Model.DTO
{
    public class UserDTO
    {
        //  REGISTER
        public class Create
        {
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

            [Required, MaxLength(100)]
            public string ImageUrl { get; set; }

            public string Role { get; set; }
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
            public DoctorDTO.Response? DoctorProfile { get; set; }
            public ReceptionistDTO.Response? ReceptionistProfile { get; set; }

        }
        public class RefreshRequest { public string RefreshToken { get; set; } }

    }
}