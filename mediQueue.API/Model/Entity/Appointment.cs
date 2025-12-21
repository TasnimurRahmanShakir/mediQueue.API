namespace mediQueue.API.Model.Entity
{
    public class Appointment
    {
        public Guid Id { get; set; }

        public Guid DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }

        public string? Reason { get; set; }
        public string Status { get; set; } = "pending";

        public DateOnly? AppointmentDate { get; set; }
        public TimeOnly? AppointmentTime { get; set; }

        public DateTime CreatedAt { get; set; }



        public ICollection<Invoice> Invoices { get; set; }
    }
}
