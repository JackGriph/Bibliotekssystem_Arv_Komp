using Bunit;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using Bibliotekssystem_Arv_Komp.Web.Components.Pages;
using Bibliotekssystem_Arv_Komp.Web.Dtos;

namespace Bibliotekssystem_Arv_Komp.Test;

public class BlazorComponentTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHandler;
    private readonly HttpClient _httpClient;

    public BlazorComponentTests()
    {
        _mockHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHandler) { BaseAddress = new Uri("http://localhost") };
        Services.AddSingleton(_httpClient);
    }

    [Fact]
    public void Books_ShouldRenderBookList()
    {
        // Arrange
        var books = new List<BookDto>
        {
            new(1, "978-91-0-012345-6", "Clean Code", "Robert Martin", 2008, 464, true),
            new(2, "978-91-0-054321-0", "Design Patterns", "GoF", 1994, 395, false)
        };
        _mockHandler.SetResponse("api/books", books);

        // Act
        var cut = Render<Books>();
        cut.WaitForState(() => cut.FindAll("tr").Count > 1);

        // Assert
        var rows = cut.FindAll("tbody tr");
        Assert.Equal(2, rows.Count);
        Assert.Contains("Clean Code", rows[0].InnerHtml);
        Assert.Contains("Design Patterns", rows[1].InnerHtml);
    }

    [Fact]
    public void Books_ShouldFilterBySearchTerm()
    {
        // Arrange
        var books = new List<BookDto>
        {
            new(1, "111", "Clean Code", "Robert Martin", 2008, 464, true),
            new(2, "222", "Harry Potter", "J.K. Rowling", 1997, 223, true),
            new(3, "333", "Clean Architecture", "Robert Martin", 2017, 432, true)
        };
        _mockHandler.SetResponse("api/books", books);

        var cut = Render<Books>();
        cut.WaitForState(() => cut.FindAll("tbody tr").Count == 3);

        // Act
        var searchInput = cut.Find("input[type='text']");
        searchInput.Input("Harry");

        // Assert
        var rows = cut.FindAll("tbody tr");
        Assert.Single(rows);
        Assert.Contains("Harry Potter", rows[0].InnerHtml);
    }

    [Fact]
    public void Members_ShouldRenderMemberList()
    {
        // Arrange
        var members = new List<MemberDto>
        {
            new(1, "M001", "Anna Andersson", "anna@test.com", DateTime.Now, 2),
            new(2, "M002", "Erik Svensson", "erik@test.com", DateTime.Now, 0)
        };
        _mockHandler.SetResponse("api/members", members);

        // Act
        var cut = Render<Members>();
        cut.WaitForState(() => cut.FindAll("tbody tr").Count > 0);

        // Assert
        var rows = cut.FindAll("tbody tr");
        Assert.Equal(2, rows.Count);
        Assert.Contains("Anna Andersson", rows[0].InnerHtml);
        Assert.Contains("Erik Svensson", rows[1].InnerHtml);
    }
}

/// <summary>
/// Enkel mock-handler for HttpClient som returnerar fordefinierade JSON-svar.
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, string> _responses = new();

    public void SetResponse<T>(string url, T content)
    {
        _responses[url] = JsonSerializer.Serialize(content);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.PathAndQuery.TrimStart('/') ?? "";
        if (_responses.TryGetValue(path, out var json))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            });
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}
