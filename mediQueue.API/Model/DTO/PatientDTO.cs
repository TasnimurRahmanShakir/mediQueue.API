using System.ComponentModel.DataAnnotations;

namespace mediQueue.API.Model.DTO
{
    public class PatientDTO
    {
        // ===================
        // 1. CREATE
        // ===================
        public class Create
        {
            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            [Required]
            [Phone]
            [StringLength(20)]
            public string PhoneNumber { get; set; }

            [StringLength(5)]
            public string? BloodGroup { get; set; }

            [Required]
            public DateTime DOB { get; set; }
        }

        // =================
        // 2. UPDATE (Put)
        // =================
        public class Update
        {
            [Required]
            public Guid Id { get; set; }

            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            [Required]
            [Phone]
            [StringLength(20)]
            public string PhoneNumber { get; set; }

            [StringLength(5)]
            public string? BloodGroup { get; set; }

            [Required]
            public DateTime DOB { get; set; }
        }

        // ==================
        // 3. RESPONSE 
        // ==================
        public class Response
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string? BloodGroup { get; set; }
            public DateTime DOB { get; set; }
            public DateTime CreatedAt { get; set; }

            public int Age
            {
                get
                {
                    var today = DateTime.UtcNow;
                    var age = today.Year - DOB.Year;
                    if (DOB.Date > today.AddYears(-age)) age--;
                    return age;
                }
            }
        }
    }
}