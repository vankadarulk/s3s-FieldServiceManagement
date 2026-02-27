namespace FieldServiceManagement.Core.DTOs;

public class SyncResultDto
{
    public int ItemsPushed { get; set; }
    public int ItemsPulled { get; set; }
    public int Conflicts { get; set; }
    public DateTime LastSyncTimestamp { get; set; }
}

public class SyncPushDto
{
    public List<JobDto> Jobs { get; set; } = new();
    public List<WorkNoteDto> WorkNotes { get; set; } = new();
    public DateTime ClientTimestamp { get; set; }
}

public class SyncPullRequestDto
{
    public DateTime LastSyncTimestamp { get; set; }
}

public class SyncPullResponseDto
{
    public List<JobDto> Jobs { get; set; } = new();
    public List<WorkNoteDto> WorkNotes { get; set; } = new();
    public DateTime ServerTimestamp { get; set; }
}
