using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldServiceManagement.API.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (role == nameof(UserRole.Technician) && Guid.TryParse(userId, out var techId))
        {
            var techJobs = await _jobService.GetByTechnicianIdAsync(techId);
            return Ok(techJobs);
        }

        var jobs = await _jobService.GetAllAsync();
        return Ok(jobs);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var job = await _jobService.GetByIdAsync(id);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] JobCreateDto dto)
    {
        var job = await _jobService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] JobUpdateDto dto)
    {
        var job = await _jobService.UpdateAsync(id, dto);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _jobService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] JobStatus status)
    {
        var job = await _jobService.UpdateStatusAsync(id, status);
        if (job == null) return NotFound();
        return Ok(job);
    }
}
