using FluentAssertions;
using Microsoft.Playwright;

namespace FieldServiceManagement.BlazorAdmin.Tests;

public class CounterPageTests : PlaywrightTestBase
{
    [Fact]
    public async Task CounterPage_ShowsInitialCountOfZero()
    {
        await Page.GotoAsync($"{BaseUrl}/counter");

        var status = await Page.Locator("[role='status']").TextContentAsync();

        status.Should().Contain("0");
    }

    [Fact]
    public async Task CounterPage_IncrementsCountOnButtonClick()
    {
        await Page.GotoAsync($"{BaseUrl}/counter");

        // Wait for Blazor JavaScript to initialize
        await Page.WaitForFunctionAsync("() => window.Blazor !== undefined && window.Blazor._internal !== undefined");

        // Wait for the Blazor Server SignalR circuit to finish connecting
        await Task.Delay(1000);

        await Page.Locator("button.btn-primary").ClickAsync();

        // Wait for Blazor Server to process the click and update the DOM
        await Page.Locator("[role='status']").Filter(new() { HasText = "1" })
            .WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });

        var status = await Page.Locator("[role='status']").TextContentAsync();

        status.Should().Contain("1");
    }

    [Fact]
    public async Task CounterPage_HasCorrectTitle()
    {
        await Page.GotoAsync($"{BaseUrl}/counter");

        var title = await Page.TitleAsync();

        title.Should().Contain("Counter");
    }
}
