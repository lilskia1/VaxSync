using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VaxSync.Web.Models;

namespace VaxSync.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Students and related records
        public DbSet<Student> Students => Set<Student>();
        public DbSet<School> Schools => Set<School>();
        public DbSet<VaccineRecord> VaccineRecords => Set<VaccineRecord>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        // New reference tables
        public DbSet<Vaccine> Vaccines => Set<Vaccine>();
        public DbSet<VaccineSchedule> VaccineSchedules => Set<VaccineSchedule>();
    }
}
