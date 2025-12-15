using System.ComponentModel.DataAnnotations;

namespace mediQueue.API.Model.DTO
{
    public class ReceptionistDTO
    {
        public class Create
        {
            [Required]
            public Guid UserId { get; set; }


            [Required]
            public string ShiftTime { get; set; }
        }

        public class Update
        {
            [Required]
            public Guid Id { get; set; }
            public string ShiftTime { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public string ShiftTime { get; set; }

            // Flattened Data

        }
    }
}