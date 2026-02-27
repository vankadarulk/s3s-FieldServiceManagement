using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.API.Controllers;

[ApiController]
[Route("api/sync")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }

    [HttpPost("push")]
    public async Task<IActionResult> Push([FromBody] SyncPushDto pushData)
    {
        var result = await _syncService.PushChangesAsync(pushData);
        return Ok(result);
    }

    [HttpPost("pull")]
    public async Task<IActionResult> Pull([FromBody] SyncPullRequestDto request)
    {
        var result = await _syncService.PullChangesAsync(request.LastSyncTimestamp);
        return Ok(result);
    }
}
