using TodoPortal.Api.Middleware;
using TodoPortal.Api.Time;
using TodoPortal.Application.Abstractions;
using TodoPortal.Application.UseCases.Todos.CreateTodo;
using TodoPortal.Application.UseCases.Todos.DeleteTodo;
using TodoPortal.Application.UseCases.Todos.GetTodoById;
using TodoPortal.Application.UseCases.Todos.GetTodos;
using TodoPortal.Application.UseCases.Todos.PatchTodo;
using TodoPortal.Application.UseCases.Todos.ReplaceTodo;
using TodoPortal.Application.UseCases.Users.GetUserById;
using TodoPortal.Application.UseCases.Users.GetUserTodos;
using TodoPortal.Application.UseCases.Users.GetUsers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IClock, SystemClock>();

builder.Services.AddScoped<GetTodosHandler>();
builder.Services.AddScoped<GetTodoByIdHandler>();
builder.Services.AddScoped<CreateTodoHandler>();
builder.Services.AddScoped<ReplaceTodoHandler>();
builder.Services.AddScoped<PatchTodoHandler>();
builder.Services.AddScoped<DeleteTodoHandler>();
builder.Services.AddScoped<GetUsersHandler>();
builder.Services.AddScoped<GetUserByIdHandler>();
builder.Services.AddScoped<GetUserTodosHandler>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
