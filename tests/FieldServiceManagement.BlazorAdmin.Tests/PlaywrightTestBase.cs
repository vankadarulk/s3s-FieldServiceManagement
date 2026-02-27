using System.Net;
using System.Net.Sockets;
using FieldServiceManagement.BlazorAdmin.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace FieldServiceManagement.BlazorAdmin.Tests;

public abstract class PlaywrightTestBase : IAsyncLifetime
{
    private WebApplication? _app;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    protected IPage Page { get; private set; } = null!;
    protected string BaseUrl { get; private set; } = string.Empty;

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public async Task InitializeAsync()
    {
        var port = GetFreePort();
        BaseUrl = $"http://127.0.0.1:{port}";

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development",
            Args = new[] { "--urls", BaseUrl }
        });
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        _app = builder.Build();
        _app.UseStaticFiles();
        _app.UseAntiforgery();
        _app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        await _app.StartAsync();

        _playwright = await Playwright.CreateAsync();

        // Use the system Chromium path if provided, otherwise fall back to Playwright's default
        var executablePath = Environment.GetEnvironmentVariable("PLAYWRIGHT_CHROMIUM_EXECUTABLE_PATH");

        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            ExecutablePath = executablePath,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        });

        Page = await _browser.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null)
            await _browser.CloseAsync();

        _playwright?.Dispose();

        if (_app is not null)
            await _app.StopAsync();
    }
}
