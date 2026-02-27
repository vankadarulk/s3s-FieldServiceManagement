namespace FieldServiceManagement.Core.Models;

public class Attachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkNoteId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public WorkNote? WorkNote { get; set; }
}
