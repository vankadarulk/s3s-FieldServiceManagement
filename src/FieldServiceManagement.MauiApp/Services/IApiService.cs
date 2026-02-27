using FieldServiceManagement.Core.DTOs;

namespace FieldServiceManagement.MauiApp.Services;

public interface IApiService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<IEnumerable<JobDto>> GetJobsAsync();
    Task<JobDto?> GetJobAsync(Guid id);
    Task<bool> UpdateJobStatusAsync(Guid id, Core.Enums.JobStatus status);
    Task<WorkNoteDto?> AddWorkNoteAsync(WorkNoteCreateDto dto);
}
