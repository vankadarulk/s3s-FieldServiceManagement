using FluentAssertions;

namespace FieldServiceManagement.BlazorAdmin.Tests;

public class HomePageTests : PlaywrightTestBase
{
    [Fact]
    public async Task HomePage_ShowsHelloWorldHeading()
    {
        await Page.GotoAsync(BaseUrl);

        var heading = await Page.Locator("h1").TextContentAsync();

        heading.Should().Contain("Hello");
    }

    [Fact]
    public async Task HomePage_HasCorrectTitle()
    {
        await Page.GotoAsync(BaseUrl);

        var title = await Page.TitleAsync();

        title.Should().Contain("Home");
    }

    [Fact]
    public async Task HomePage_ShowsNavigationLinks()
    {
        await Page.GotoAsync(BaseUrl);

        var navLinks = Page.Locator("nav .nav-link");
        var count = await navLinks.CountAsync();

        count.Should().BeGreaterThan(0);
    }
}
