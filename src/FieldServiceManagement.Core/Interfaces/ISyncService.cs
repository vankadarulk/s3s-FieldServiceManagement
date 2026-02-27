using FieldServiceManagement.Core.DTOs;

namespace FieldServiceManagement.Core.Interfaces;

/// <summary>Defines offline synchronization operations.</summary>
public interface ISyncService
{
    Task<SyncResultDto> PushChangesAsync(SyncPushDto pushData);
    Task<SyncPullResponseDto> PullChangesAsync(DateTime lastSyncTimestamp);
    Task<SyncResultDto> GetSyncStatusAsync();
}
