namespace mediQueue.API.Model.Entity
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public Decimal? Amount { get; set; }
        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
