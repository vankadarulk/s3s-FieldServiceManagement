using FieldServiceManagement.API.Data;
using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FieldServiceManagement.API.Controllers;

[ApiController]
[Route("api/worknotes")]
[Authorize]
public class WorkNotesController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkNotesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetByJob(Guid jobId)
    {
        var notes = await _context.WorkNotes
            .Include(wn => wn.Author)
            .Where(wn => wn.JobId == jobId)
            .Select(wn => new WorkNoteDto
            {
                Id = wn.Id,
                JobId = wn.JobId,
                AuthorId = wn.AuthorId,
                AuthorName = wn.Author != null ? $"{wn.Author.FirstName} {wn.Author.LastName}" : string.Empty,
                Content = wn.Content,
                CreatedAt = wn.CreatedAt
            })
            .ToListAsync();
        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkNoteCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var authorId))
            return Unauthorized();

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role == nameof(UserRole.Technician))
        {
            var job = await _context.Jobs.FindAsync(dto.JobId);
            if (job == null || job.AssignedTechnicianId != authorId)
                return Forbid();
        }

        var note = new WorkNote
        {
            JobId = dto.JobId,
            AuthorId = authorId,
            Content = dto.Content
        };
        _context.WorkNotes.Add(note);
        await _context.SaveChangesAsync();
        return Ok(new WorkNoteDto
        {
            Id = note.Id,
            JobId = note.JobId,
            AuthorId = note.AuthorId,
            Content = note.Content,
            CreatedAt = note.CreatedAt
        });
    }
}
