using FieldServiceManagement.API.Data;
using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.API.Services;

public class SyncService : ISyncService
{
    private readonly AppDbContext _context;
    private readonly IJobService _jobService;

    public SyncService(AppDbContext context, IJobService jobService)
    {
        _context = context;
        _jobService = jobService;
    }

    public async Task<SyncResultDto> PushChangesAsync(SyncPushDto pushData)
    {
        int pushed = 0;
        int conflicts = 0;

        foreach (var jobDto in pushData.Jobs)
        {
            var existing = await _context.Jobs.IgnoreQueryFilters()
                .FirstOrDefaultAsync(j => j.Id == jobDto.Id);

            if (existing == null)
                continue;

            if (jobDto.UpdatedAt >= existing.UpdatedAt)
            {
                existing.Title = jobDto.Title;
                existing.Description = jobDto.Description;
                existing.Status = jobDto.Status;
                existing.Priority = jobDto.Priority;
                existing.AssignedTechnicianId = jobDto.AssignedTechnicianId;
                existing.ScheduledDate = jobDto.ScheduledDate;
                existing.Location = jobDto.Location;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.Version++;
                pushed++;
            }
            else
            {
                conflicts++;
            }
        }

        await _context.SaveChangesAsync();

        return new SyncResultDto
        {
            ItemsPushed = pushed,
            ItemsPulled = 0,
            Conflicts = conflicts,
            LastSyncTimestamp = DateTime.UtcNow
        };
    }

    public async Task<SyncPullResponseDto> PullChangesAsync(DateTime lastSyncTimestamp)
    {
        var jobs = await _context.Jobs
            .Include(j => j.AssignedTechnician)
            .Include(j => j.Customer)
            .Where(j => j.UpdatedAt > lastSyncTimestamp)
            .Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Status = j.Status,
                Priority = j.Priority,
                AssignedTechnicianId = j.AssignedTechnicianId,
                TechnicianName = j.AssignedTechnician != null
                    ? $"{j.AssignedTechnician.FirstName} {j.AssignedTechnician.LastName}"
                    : null,
                CustomerId = j.CustomerId,
                CustomerName = j.Customer != null ? j.Customer.Name : null,
                ScheduledDate = j.ScheduledDate,
                CompletedDate = j.CompletedDate,
                Location = j.Location,
                Version = j.Version,
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt
            })
            .ToListAsync();

        var workNotes = await _context.WorkNotes
            .Include(wn => wn.Author)
            .Where(wn => wn.CreatedAt > lastSyncTimestamp)
            .Select(wn => new WorkNoteDto
            {
                Id = wn.Id,
                JobId = wn.JobId,
                AuthorId = wn.AuthorId,
                AuthorName = wn.Author != null
                    ? $"{wn.Author.FirstName} {wn.Author.LastName}"
                    : string.Empty,
                Content = wn.Content,
                CreatedAt = wn.CreatedAt
            })
            .ToListAsync();

        return new SyncPullResponseDto
        {
            Jobs = jobs,
            WorkNotes = workNotes,
            ServerTimestamp = DateTime.UtcNow
        };
    }

    public Task<SyncResultDto> GetSyncStatusAsync()
    {
        return Task.FromResult(new SyncResultDto
        {
            LastSyncTimestamp = DateTime.UtcNow
        });
    }
}
