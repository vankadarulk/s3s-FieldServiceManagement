using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Interfaces;

namespace FieldServiceManagement.MauiApp.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _isLoggedIn;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Login";
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    public LoginResponseDto? CurrentUser { get; private set; }

    public async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email and password are required.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var response = await _authService.LoginAsync(new LoginRequestDto
            {
                Email = Email,
                Password = Password
            });

            if (response != null)
            {
                CurrentUser = response;
                IsLoggedIn = true;
            }
            else
            {
                ErrorMessage = "Invalid credentials. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
