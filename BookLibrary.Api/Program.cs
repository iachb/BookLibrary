using BookLibrary.Api.Profiles;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Infrastructure.Data;
using BookLibrary.Infrastructure.Services;
using BookLibrary.Infrastructure.Repository;    
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BookLibrary.Core.Profiles;

var builder = WebApplication.CreateBuilder(args);

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

app.UseMiddleware<BookLibrary.Api.Middleware.GlobalExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BookLibrary");
    });
}

// Add CORS
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
