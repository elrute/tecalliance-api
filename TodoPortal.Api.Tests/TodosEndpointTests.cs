using System.Net;
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
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(todo);
        Assert.Equal("integration test", todo!.Title);
    }

    [Fact]
    public async Task GetTodoById_Should_Return_Item()
    {
        var todo = await _client.GetFromJsonAsync<TodoResponse>("/todos/1");
        Assert.NotNull(todo);
        Assert.Equal(1, todo!.Id);
    }

    [Fact]
    public async Task ReplaceTodo_Should_Update_Title()
    {
        var created = await CreateTodoAsync("replace-me");
        var replaceRequest = new ReplaceTodoRequest(created.UserId, "updated title", created.Completed);

        var response = await _client.PutAsJsonAsync($"/todos/{created.Id}", replaceRequest);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<TodoResponse>();

        Assert.NotNull(updated);
        Assert.Equal("updated title", updated!.Title);
    }

    [Fact]
    public async Task PatchTodo_Should_Update_Flags()
    {
        var created = await CreateTodoAsync("patch-me");
        var patchRequest = new PatchTodoRequest(created.UserId, null, true);

        var response = await _client.PatchAsJsonAsync($"/todos/{created.Id}", patchRequest);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<TodoResponse>();

        Assert.NotNull(updated);
        Assert.True(updated!.Completed);
    }

    [Fact]
    public async Task DeleteTodo_Should_Remove_Item()
    {
        var created = await CreateTodoAsync("delete-me");

        var deleteResponse = await _client.DeleteAsync($"/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<TodoResponse> CreateTodoAsync(string title)
    {
        var request = new CreateTodoRequest(1, title, false);
        var response = await _client.PostAsJsonAsync("/todos", request);
        response.EnsureSuccessStatusCode();
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(todo);
        return todo!;
    }

    //a new local definition instead of referencing the dtos in the application lib allows us to rely fully on the json
    //to detect breaking changes
    private sealed record TodoResponse(int UserId, int Id, string Title, bool Completed);
}
