namespace FieldServiceManagement.Core.Models;

public class WorkNote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public Job? Job { get; set; }
    public User? Author { get; set; }
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
