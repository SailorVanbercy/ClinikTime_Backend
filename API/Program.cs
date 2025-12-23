using ClinikTime.service;
using ClinikTime.utils.PasswordHasher;
using Infrastructure.Data;
using Infrastructure.user.EF;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ======================
// Services
// ======================

// Controllers
builder.Services.AddControllers();

// Swagger (Swashbuckle UNIQUEMENT – OK en .NET 9)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<ClinikTimeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ClinikTime")
    )
);

// ======================
// Dependency Injection
// ======================
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

// ======================
// HTTP Pipeline
// ======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "ClinikTime API";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClinikTime API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();