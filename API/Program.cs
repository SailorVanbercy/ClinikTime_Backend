using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ======================
// Services
// ======================
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ClinikTimeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ClinikTime")
    )
);

var app = builder.Build();

// ======================
// Pipeline HTTP
// ======================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();