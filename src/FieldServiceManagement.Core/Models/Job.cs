using FieldServiceManagement.Core.Enums;

namespace FieldServiceManagement.Core.Models;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Created;
    public JobPriority Priority { get; set; } = JobPriority.Medium;
    public Guid? AssignedTechnicianId { get; set; }
    public Guid? CustomerId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Address? Location { get; set; }
    public bool IsDeleted { get; set; }
    public long Version { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? AssignedTechnician { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<WorkNote> WorkNotes { get; set; } = new List<WorkNote>();
}
