using System;
using System.Threading.Tasks;
using VaxSync.Web.Data;
using VaxSync.Web.Models;

namespace VaxSync.Web.Services
{
    public class AuditLogService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string userName, string action)
        {
            var log = new AuditLog
            {
                User = userName,
                Action = action,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
