using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TodoPortal.Api.Tests;

public sealed class UsersEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UsersEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUsers_Should_Filter_By_Email()
    {
        var users = await _client.GetFromJsonAsync<List<UserResponse>>("/users?email=Sincere@april.biz");
        Assert.NotNull(users);
        Assert.Single(users!);
        Assert.Equal("Sincere@april.biz", users![0].Email);
    }

    [Fact]
    public async Task GetUserById_Should_Return_User()
    {
        var user = await _client.GetFromJsonAsync<UserResponse>("/users/1");
        Assert.NotNull(user);
        Assert.Equal("Bret", user!.Username);
    }

    [Fact]
    public async Task GetUserTodos_Should_Return_Items()
    {
        var todos = await _client.GetFromJsonAsync<List<TodoResponse>>("/users/1/todos");
        Assert.NotNull(todos);
        Assert.NotEmpty(todos!);
        Assert.All(todos!, t => Assert.Equal(1, t.UserId));
    }

    //a new local definition instead of referencing the dtos in the application lib allows us to rely fully on the json
    //to detect breaking changes
    private sealed record UserResponse(int Id, string Username, string Email);
    private sealed record TodoResponse(int UserId, int Id, string Title, bool Completed);
}
