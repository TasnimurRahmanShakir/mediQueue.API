namespace mediQueue.API.Model.Entity
{
    public class Receptionist
    {
        public Guid Id { get; set; }

        public string ShiftTime { get; set; }
        public DateTime? SignInTime { get; set; }
        public DateTime? SignOutTime { get; set; }

        // Foreign Key to User
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}