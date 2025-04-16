using AppointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)   
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData
            (
                new User
                {
                    UserId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Password = "admin@123",
                    Role = "Admin",
                    FullName = "System Admin",
                    Email = "admin@hospital.com",
                    Phone = "1234567890"
                }
            );

            // Appointment relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //// Configure Time as string
            //modelBuilder.Entity<Appointment>()
            //    .Property(a => a.AppointmentTime)
            //    .HasConversion(
            //        v => v.ToString(),
            //        v => v);
        }

    }
}
