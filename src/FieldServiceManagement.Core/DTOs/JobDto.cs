using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.Core.Models;

namespace FieldServiceManagement.Core.DTOs;

public class JobDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JobStatus Status { get; set; }
    public JobPriority Priority { get; set; }
    public Guid? AssignedTechnicianId { get; set; }
    public string? TechnicianName { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Address? Location { get; set; }
    public long Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class JobCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JobPriority Priority { get; set; } = JobPriority.Medium;
    public Guid? AssignedTechnicianId { get; set; }
    public Guid? CustomerId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public Address? Location { get; set; }
}

public class JobUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JobStatus Status { get; set; }
    public JobPriority Priority { get; set; }
    public Guid? AssignedTechnicianId { get; set; }
    public Guid? CustomerId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public Address? Location { get; set; }
    public long Version { get; set; }
}
