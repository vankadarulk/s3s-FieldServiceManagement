using FieldServiceManagement.API.Data;
using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.Core.Interfaces;
using FieldServiceManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.API.Services;

public class JobService : IJobService
{
    private readonly AppDbContext _context;

    public JobService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobDto>> GetAllAsync()
    {
        return await _context.Jobs
            .Include(j => j.AssignedTechnician)
            .Include(j => j.Customer)
            .Select(j => MapToDto(j))
            .ToListAsync();
    }

    public async Task<JobDto?> GetByIdAsync(Guid id)
    {
        var job = await _context.Jobs
            .Include(j => j.AssignedTechnician)
            .Include(j => j.Customer)
            .Include(j => j.WorkNotes).ThenInclude(wn => wn.Author)
            .FirstOrDefaultAsync(j => j.Id == id);
        return job == null ? null : MapToDto(job);
    }

    public async Task<JobDto> CreateAsync(JobCreateDto dto)
    {
        var job = new Job
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            AssignedTechnicianId = dto.AssignedTechnicianId,
            CustomerId = dto.CustomerId,
            ScheduledDate = dto.ScheduledDate,
            Location = dto.Location,
            Status = dto.AssignedTechnicianId.HasValue ? JobStatus.Assigned : JobStatus.Created
        };
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return MapToDto(job);
    }

    public async Task<JobDto?> UpdateAsync(Guid id, JobUpdateDto dto)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return null;

        job.Title = dto.Title;
        job.Description = dto.Description;
        job.Status = dto.Status;
        job.Priority = dto.Priority;
        job.AssignedTechnicianId = dto.AssignedTechnicianId;
        job.CustomerId = dto.CustomerId;
        job.ScheduledDate = dto.ScheduledDate;
        job.Location = dto.Location;
        job.UpdatedAt = DateTime.UtcNow;
        job.Version++;

        await _context.SaveChangesAsync();
        return MapToDto(job);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return false;
        job.IsDeleted = true;
        job.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<JobDto>> GetByTechnicianIdAsync(Guid technicianId)
    {
        return await _context.Jobs
            .Include(j => j.AssignedTechnician)
            .Include(j => j.Customer)
            .Where(j => j.AssignedTechnicianId == technicianId)
            .Select(j => MapToDto(j))
            .ToListAsync();
    }

    public async Task<JobDto?> UpdateStatusAsync(Guid id, JobStatus status)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return null;
        job.Status = status;
        if (status == JobStatus.Completed)
            job.CompletedDate = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;
        job.Version++;
        await _context.SaveChangesAsync();
        return MapToDto(job);
    }

    private static JobDto MapToDto(Job job) => new()
    {
        Id = job.Id,
        Title = job.Title,
        Description = job.Description,
        Status = job.Status,
        Priority = job.Priority,
        AssignedTechnicianId = job.AssignedTechnicianId,
        TechnicianName = job.AssignedTechnician != null
            ? $"{job.AssignedTechnician.FirstName} {job.AssignedTechnician.LastName}"
            : null,
        CustomerId = job.CustomerId,
        CustomerName = job.Customer?.Name,
        ScheduledDate = job.ScheduledDate,
        CompletedDate = job.CompletedDate,
        Location = job.Location,
        Version = job.Version,
        CreatedAt = job.CreatedAt,
        UpdatedAt = job.UpdatedAt
    };
}
