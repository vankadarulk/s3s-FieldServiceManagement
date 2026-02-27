using FieldServiceManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<WorkNote> WorkNotes => Set<WorkNote>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsOne(e => e.Address);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.OwnsOne(e => e.Location);
            entity.HasQueryFilter(e => !e.IsDeleted);
            entity.HasOne(e => e.AssignedTechnician)
                .WithMany()
                .HasForeignKey(e => e.AssignedTechnicianId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<WorkNote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(e => !e.IsDeleted);
            entity.HasOne(e => e.Job)
                .WithMany(j => j.WorkNotes)
                .HasForeignKey(e => e.JobId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Author)
                .WithMany()
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.WorkNote)
                .WithMany(w => w.Attachments)
                .HasForeignKey(e => e.WorkNoteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
