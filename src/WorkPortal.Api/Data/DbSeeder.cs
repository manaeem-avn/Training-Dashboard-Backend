using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Data;

public static class DbSeeder
{
    public static async Task Seed(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Users.AnyAsync()) return;

        PasswordHasher<AppUser> hasher = new();

        AppUser admin = new() { FullName = "Amelia Admin", Email = "admin@workportal.com", Role = "Admin" };
        AppUser alex = new() { FullName = "Alex Developer", Email = "alex@workportal.com", Role = "User" };
        AppUser sam = new() { FullName = "Sam Tester", Email = "sam@workportal.com", Role = "User" };

        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");
        alex.PasswordHash = hasher.HashPassword(alex, "User1234!");
        sam.PasswordHash = hasher.HashPassword(sam, "User1234!");

        db.Users.AddRange(admin, alex, sam);
        await db.SaveChangesAsync();

        Project portal = new()
        {
            Name = "Customer Portal Revamp",
            Description = "Rebuild the customer facing portal on Angular and .NET 8.",
            Status = "Active",
            StartDate = DateTime.UtcNow.Date.AddDays(-30),
            OwnerId = admin.Id,
            CreatedBy = admin.Email
        };

        Project migration = new()
        {
            Name = "Database Migration",
            Description = "Move the legacy database to SQL Server with clean schema and indexes.",
            Status = "OnHold",
            StartDate = DateTime.UtcNow.Date.AddDays(-60),
            EndDate = DateTime.UtcNow.Date.AddDays(30),
            OwnerId = alex.Id,
            CreatedBy = admin.Email
        };

        Project mobile = new()
        {
            Name = "Mobile App Pilot",
            Description = "Pilot a mobile client that reuses the same Web API.",
            Status = "Completed",
            StartDate = DateTime.UtcNow.Date.AddDays(-120),
            EndDate = DateTime.UtcNow.Date.AddDays(-10),
            OwnerId = admin.Id,
            CreatedBy = admin.Email
        };

        db.Projects.AddRange(portal, migration, mobile);
        await db.SaveChangesAsync();

        db.Tasks.AddRange(
            new WorkTask { Title = "Design the login screen", Description = "Reactive form with validation.", Status = "Done", Priority = "High", ProjectId = portal.Id, AssigneeId = alex.Id, DueDate = DateTime.UtcNow.Date.AddDays(-14), CreatedBy = admin.Email },
            new WorkTask { Title = "Build the dashboard cards", Description = "Summary counts and a chart.", Status = "InProgress", Priority = "High", ProjectId = portal.Id, AssigneeId = alex.Id, DueDate = DateTime.UtcNow.Date.AddDays(5), CreatedBy = admin.Email },
            new WorkTask { Title = "Write API integration tests", Description = "Cover auth and projects.", Status = "Todo", Priority = "Medium", ProjectId = portal.Id, AssigneeId = sam.Id, DueDate = DateTime.UtcNow.Date.AddDays(9), CreatedBy = admin.Email },
            new WorkTask { Title = "Fix the pagination defect", Description = "Page size cap is ignored.", Status = "Todo", Priority = "High", ProjectId = portal.Id, AssigneeId = sam.Id, DueDate = DateTime.UtcNow.Date.AddDays(-2), CreatedBy = admin.Email },
            new WorkTask { Title = "Normalise the legacy schema", Description = "Third normal form.", Status = "InProgress", Priority = "Medium", ProjectId = migration.Id, AssigneeId = alex.Id, DueDate = DateTime.UtcNow.Date.AddDays(12), CreatedBy = admin.Email },
            new WorkTask { Title = "Add indexes for slow queries", Description = "Check the execution plans.", Status = "Todo", Priority = "Low", ProjectId = migration.Id, DueDate = DateTime.UtcNow.Date.AddDays(20), CreatedBy = admin.Email },
            new WorkTask { Title = "Prepare the rollback script", Description = "Restore from the last backup.", Status = "Todo", Priority = "High", ProjectId = migration.Id, AssigneeId = sam.Id, CreatedBy = admin.Email },
            new WorkTask { Title = "Ship the pilot build", Description = "Internal testers only.", Status = "Done", Priority = "Medium", ProjectId = mobile.Id, AssigneeId = alex.Id, DueDate = DateTime.UtcNow.Date.AddDays(-15), CreatedBy = admin.Email },
            new WorkTask { Title = "Collect pilot feedback", Description = "Survey the testers.", Status = "Done", Priority = "Low", ProjectId = mobile.Id, AssigneeId = sam.Id, DueDate = DateTime.UtcNow.Date.AddDays(-11), CreatedBy = admin.Email });

        await db.SaveChangesAsync();

        int firstTaskId = await db.Tasks.OrderBy(t => t.Id).Select(t => t.Id).FirstAsync();

        db.Comments.AddRange(
            new Comment { Text = "Approved by design, please proceed.", TaskId = firstTaskId, AuthorId = admin.Id },
            new Comment { Text = "Validation messages are wired up now.", TaskId = firstTaskId, AuthorId = alex.Id });

        await db.SaveChangesAsync();
    }
}
