using TodoPortal.Api.Middleware;
using TodoPortal.Application.UseCases.Todos.CreateTodo;
using TodoPortal.Application.UseCases.Todos.DeleteTodo;
using TodoPortal.Application.UseCases.Todos.GetTodoById;
using TodoPortal.Application.UseCases.Todos.GetTodos;
using TodoPortal.Application.UseCases.Todos.PatchTodo;
using TodoPortal.Application.UseCases.Todos.ReplaceTodo;
using TodoPortal.Application.UseCases.Users.GetUserById;
using TodoPortal.Application.UseCases.Users.GetUserTodos;
using TodoPortal.Application.UseCases.Users.GetUsers;
using TodoPortal.Domain.Ports;
using TodoPortal.Infrastructure.DataLoading;
using TodoPortal.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<JsonPlaceholderDataStore>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();

builder.Services.AddScoped<GetTodosHandler>();
builder.Services.AddScoped<GetTodoByIdHandler>();
builder.Services.AddScoped<CreateTodoHandler>();
builder.Services.AddScoped<ReplaceTodoHandler>();
builder.Services.AddScoped<PatchTodoHandler>();
builder.Services.AddScoped<DeleteTodoHandler>();
builder.Services.AddScoped<GetUsersHandler>();
builder.Services.AddScoped<GetUserByIdHandler>();
builder.Services.AddScoped<GetUserTodosHandler>();

var frontendOrigin = builder.Configuration.GetValue<string>("Cors:Frontend") ?? "http://localhost:4200";

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
        policy.WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("DevCors");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
