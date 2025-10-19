using Microsoft.EntityFrameworkCore;

namespace VaxSync.Web.Data;

public class DevSeeder
{
    private readonly ApplicationDbContext _db;

    public DevSeeder(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(int targetStudentCount, int schoolCount, int batchSize)
    {
        if (targetStudentCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(targetStudentCount), "Target student count must be positive.");

        if (schoolCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(schoolCount), "School count must be positive.");

        if (batchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be positive.");

        // Fast exit if already seeded near target
        var current = await _db.Students.CountAsync();
        if (current >= targetStudentCount * 0.95) return;
    }
}
