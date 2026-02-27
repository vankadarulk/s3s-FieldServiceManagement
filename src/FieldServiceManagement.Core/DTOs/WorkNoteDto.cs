namespace FieldServiceManagement.Core.DTOs;

public class WorkNoteDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WorkNoteCreateDto
{
    public Guid JobId { get; set; }
    public string Content { get; set; } = string.Empty;
}
