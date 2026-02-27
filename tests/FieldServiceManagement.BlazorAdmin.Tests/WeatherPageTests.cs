using FluentAssertions;

namespace FieldServiceManagement.BlazorAdmin.Tests;

public class WeatherPageTests : PlaywrightTestBase
{
    [Fact]
    public async Task WeatherPage_HasCorrectTitle()
    {
        await Page.GotoAsync($"{BaseUrl}/weather");

        var title = await Page.TitleAsync();

        title.Should().Contain("Weather");
    }

    [Fact]
    public async Task WeatherPage_ShowsWeatherHeading()
    {
        await Page.GotoAsync($"{BaseUrl}/weather");

        var heading = await Page.Locator("h1").TextContentAsync();

        heading.Should().Contain("Weather");
    }

    [Fact]
    public async Task WeatherPage_ShowsWeatherTable()
    {
        await Page.GotoAsync($"{BaseUrl}/weather");

        // Wait for the table to appear after the 500ms simulated delay
        await Page.WaitForSelectorAsync("table");

        var rows = Page.Locator("table tbody tr");
        var count = await rows.CountAsync();

        count.Should().Be(5);
    }
}
