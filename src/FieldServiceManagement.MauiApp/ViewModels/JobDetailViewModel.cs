using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.MauiApp.Services;

namespace FieldServiceManagement.MauiApp.ViewModels;

public class JobDetailViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private JobDto? _job;

    public JobDetailViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Job Details";
    }

    public JobDto? Job
    {
        get => _job;
        set => SetProperty(ref _job, value);
    }

    public async Task LoadJobAsync(Guid jobId)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            Job = await _apiService.GetJobAsync(jobId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load job: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> UpdateStatusAsync(JobStatus status)
    {
        if (Job == null) return false;
        IsBusy = true;
        try
        {
            var result = await _apiService.UpdateJobStatusAsync(Job.Id, status);
            if (result && Job != null)
                Job.Status = status;
            return result;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to update status: {ex.Message}";
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
