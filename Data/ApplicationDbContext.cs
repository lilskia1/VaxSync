using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VaxSync.Web.Models;

namespace VaxSync.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tables
        public DbSet<School> Schools => Set<School>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Vaccine> Vaccines => Set<Vaccine>();
        public DbSet<VaccineSchedule> VaccineSchedules => Set<VaccineSchedule>();
        public DbSet<StudentVaccine> StudentVaccines => Set<StudentVaccine>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<StudentRequiredDose> StudentRequiredDoses => Set<StudentRequiredDose>();


        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Student>()
             .HasOne(s => s.School)
             .WithMany(x => x.Students)
             .HasForeignKey(s => s.SchoolId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<StudentVaccine>()
             .HasOne(x => x.Student)
             .WithMany(s => s.Vaccines)
             .HasForeignKey(x => x.StudentId)
             .OnDelete(DeleteBehavior.Cascade);

            b.Entity<StudentVaccine>()
             .HasOne(x => x.Vaccine)
             .WithMany()
             .HasForeignKey(x => x.VaccineId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Student>()
             .HasIndex(s => new { s.SchoolId, s.LastName, s.FirstName });
        }
    }
}
