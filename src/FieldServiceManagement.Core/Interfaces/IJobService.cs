using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;

namespace FieldServiceManagement.Core.Interfaces;

/// <summary>Defines operations for managing jobs.</summary>
public interface IJobService
{
    Task<IEnumerable<JobDto>> GetAllAsync();
    Task<JobDto?> GetByIdAsync(Guid id);
    Task<JobDto> CreateAsync(JobCreateDto dto);
    Task<JobDto?> UpdateAsync(Guid id, JobUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<JobDto>> GetByTechnicianIdAsync(Guid technicianId);
    Task<JobDto?> UpdateStatusAsync(Guid id, JobStatus status);
}
