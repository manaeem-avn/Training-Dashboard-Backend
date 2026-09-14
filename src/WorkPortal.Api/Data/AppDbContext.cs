using Microsoft.EntityFrameworkCore;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<WorkTask> Tasks => Set<WorkTask>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AppUser>(e =>
        {
            e.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(150);
            e.Property(x => x.Role).IsRequired().HasMaxLength(20);
            e.HasIndex(x => x.Email).IsUnique();
        });

        b.Entity<RefreshToken>(e =>
        {
            e.Property(x => x.Token).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Token);
            e.HasOne(x => x.User).WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Project>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.Status).IsRequired().HasMaxLength(20);
            e.HasOne(x => x.Owner).WithMany(u => u.OwnedProjects)
                .HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<WorkTask>(e =>
        {
            e.Property(x => x.Title).IsRequired().HasMaxLength(150);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.Status).IsRequired().HasMaxLength(20);
            e.Property(x => x.Priority).IsRequired().HasMaxLength(20);
            e.HasOne(x => x.Project).WithMany(p => p.Tasks)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Assignee).WithMany(u => u.AssignedTasks)
                .HasForeignKey(x => x.AssigneeId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<Comment>(e =>
        {
            e.Property(x => x.Text).IsRequired().HasMaxLength(1000);
            e.HasOne(x => x.Task).WithMany(t => t.Comments)
                .HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Author).WithMany()
                .HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Attachment>(e =>
        {
            e.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            e.Property(x => x.StoredName).IsRequired().HasMaxLength(100);
            e.Property(x => x.ContentType).HasMaxLength(100);
            e.HasOne(x => x.Task).WithMany(t => t.Attachments)
                .HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
