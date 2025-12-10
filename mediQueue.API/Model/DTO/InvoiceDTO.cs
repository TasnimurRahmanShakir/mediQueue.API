using System.ComponentModel.DataAnnotations;

namespace mediQueue.API.Model.DTO
{
    public class InvoiceDTO
    {
        public class Create
        {
            [Required]
            public Guid AppointmentId { get; set; }

            [Required]
            public decimal Amount { get; set; }

        }

        public class Response
        {
            public Guid Id { get; set; }
            public Guid AppointmentId { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }
    }
}