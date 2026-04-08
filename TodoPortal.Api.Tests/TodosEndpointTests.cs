using System.Net.Http.Json;
using TodoPortal.Api.Requests.Todos;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TodoPortal.Api.Tests;

public sealed class TodosEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodosEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_Should_Return_Data()
    {
        var response = await _client.GetAsync("/todos");
        response.EnsureSuccessStatusCode();
        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        Assert.NotNull(todos);
        Assert.NotEmpty(todos!);
    }

    [Fact]
    public async Task PostTodos_Should_Create_Item()
    {
        var request = new CreateTodoRequest(1, "integration test", false);
        var response = await _client.PostAsJsonAsync("/todos", request);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(todo);
        Assert.Equal("integration test", todo!.Title);
    }

    private sealed record TodoResponse(int UserId, int Id, string Title, bool Completed);
}
