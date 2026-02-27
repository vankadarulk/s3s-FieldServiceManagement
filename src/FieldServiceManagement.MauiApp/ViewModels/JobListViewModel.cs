using System.Collections.ObjectModel;
using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.MauiApp.Services;

namespace FieldServiceManagement.MauiApp.ViewModels;

public class JobListViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IConnectivityService _connectivityService;
    private string _searchText = string.Empty;
    private ObservableCollection<JobDto> _jobs = new();

    public JobListViewModel(IApiService apiService, IConnectivityService connectivityService)
    {
        _apiService = apiService;
        _connectivityService = connectivityService;
        Title = "My Jobs";
    }

    public ObservableCollection<JobDto> Jobs
    {
        get => _jobs;
        set => SetProperty(ref _jobs, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            FilterJobs();
        }
    }

    public bool IsConnected => _connectivityService.IsConnected;

    private List<JobDto> _allJobs = new();

    public async Task LoadJobsAsync()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var jobs = await _apiService.GetJobsAsync();
            _allJobs = jobs.ToList();
            FilterJobs();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load jobs: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void FilterJobs()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allJobs
            : _allJobs.Where(j =>
                j.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (j.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        Jobs = new ObservableCollection<JobDto>(filtered);
    }
}
