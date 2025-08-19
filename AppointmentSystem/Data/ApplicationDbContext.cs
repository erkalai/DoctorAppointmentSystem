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
                },
                new User
                {
                    UserId = new Guid("41111111-1111-1111-1111-111111111111"),
                    Password = "admin@123",
                    Role = "Receptionist",
                    FullName = "Receptionist",
                    Email = "r@hospital.com",
                    Phone = "1234567891"
                },
                 new User
                 {
                     UserId = new Guid("19111111-1111-1111-1111-111111111111"),
                     Password = "admin@123",
                     Role = "Doctor",
                     FullName = "Doctor",
                     Email = "d@hospital.com",
                     Phone = "1234567894"
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

        }

    }
}
