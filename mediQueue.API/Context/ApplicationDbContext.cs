using mediQueue.API.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace mediQueue.API.Context
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ========================
            // Patient Configeration
            //=========================

            builder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.HasIndex(p => p.PhoneNumber).IsUnique();
                entity.Property(p => p.BloodGroup).IsRequired(false).HasMaxLength(3)
                      .HasConversion(v => v == null ? null : v.ToUpper(), v => v);
                entity.ToTable(t => t.HasCheckConstraint("PatientBloodGroup",
                       "[BloodGroup] IN ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-')"));
                entity.Property(p => p.DOB).IsRequired();
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });


            // ========================
            // User Configeration
            //=========================

            builder.Entity<User>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.HasIndex(i => i.PhoneNumber).IsUnique();
                entity.Property(p => p.Email).IsRequired().HasMaxLength(30);
                entity.HasIndex(i => i.Email).IsUnique();
                entity.Property(p => p.PasswordHash).IsRequired().HasMaxLength(100);
                entity.Property(p => p.ImageUrl).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Role).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Status).HasDefaultValue("Active");

            });

            // ========================
            // Doctor Configeration
            //=========================

            builder.Entity<Doctor>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Specialization).IsRequired().HasMaxLength(100);
                entity.Property(p => p.LicenseNumber).IsRequired().HasMaxLength(30);
                entity.Property(p => p.ConsultationFee).IsRequired().HasColumnType("decimal(18,2)");
                entity.HasOne(p => p.User).WithOne(u => u.DoctorProfile)
                      .HasForeignKey<Doctor>(f => f.UserId).OnDelete(DeleteBehavior.Cascade);

            });

            // ===========================
            // Receptionist Configeration
            //============================

            builder.Entity<Receptionist>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasOne(r => r.User).WithOne(u => u.ReceptionistProfile).HasForeignKey<Receptionist>(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // ===========================
            // Appointments Configeration
            //============================

            builder.Entity<Appointment>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Reason).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Status).IsRequired().HasMaxLength(10);
                entity.ToTable(t => t.HasCheckConstraint("AppointmentStatus",
                       "[Status] in ('Pending', 'In Progress', 'Completed')"));
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");


                entity.HasOne(a => a.Patient).WithMany(p => p.Appointments).HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor).WithMany(p => p.Appointments).HasForeignKey(a => a.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // ===========================
            // Invoice Configeration
            //============================


            builder.Entity<Invoice>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.ToTable(t => t.HasCheckConstraint("InvoiceStatus",
                       "[Status] in ('Due', 'Paid')"));
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(a => a.Appointment).WithMany(m => m.Invoices)
                      .HasForeignKey(f => f.AppointmentId).OnDelete(DeleteBehavior.Restrict);

            });
        }







    }
}