namespace mediQueue.API.Model.Entity
{
    public class Patient
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? BloodGroup { get; set; }
        public DateTime DOB { get; set; }
        public DateTime CreatedAt { get; set; }


        public ICollection<Appointment> Appointments { get; set; }
    }
}
