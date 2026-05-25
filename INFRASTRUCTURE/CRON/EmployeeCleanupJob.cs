using DOMAIN.ENUM;
using INFRASTRUCTURE.DATA;

public class EmployeeCleanupJob
{
    private readonly AppDbContext _db;

    public EmployeeCleanupJob(AppDbContext db)
    {
        _db = db;
    }

    public async Task ArchiveEmployees()
    {
        // Compare full date and time in UTC to match PostgreSQL timestamptz expectations
        var nowUtc = DateTime.UtcNow;

        var employees = _db.Employees
            .Where(x =>
                x.NoticeEndDate.HasValue &&
                x.NoticeEndDate < nowUtc &&
                x.Status != EmployeeStatus.Archived)
            .ToList();

        foreach (var emp in employees)
        {
            emp.Status = EmployeeStatus.Archived;
        }

        await _db.SaveChangesAsync();
    }
}