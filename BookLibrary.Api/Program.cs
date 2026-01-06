using BookLibrary.Api.Middleware;
using BookLibrary.Api.Profiles;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Profiles;
using BookLibrary.Infrastructure.Data;
using BookLibrary.Infrastructure.Repository;
using BookLibrary.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configure logging levels (optional minimal setup)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// DB connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BookLibrary", Version = "v1" });
});

// Repository registration
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

// Service registration
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<BookProfile>();
    cfg.AddProfile<BookItemProfile>();
    cfg.AddProfile<AuthorProfile>();
    cfg.AddProfile<AuthorItemProfile>();
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Development-only request logging middleware
if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("Starting BookLibrary API in Development environment");

    app.Use(async (context, next) =>
    {
        var logger = context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("RequestLogging");

        logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            await next();
        }
        finally
        {
            sw.Stop();
            logger.LogInformation("Completed request: {StatusCode} in {Elapsed} ms", context.Response.StatusCode, sw.ElapsedMilliseconds);
        }
    });
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BookLibrary");
});

// Add CORS
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
