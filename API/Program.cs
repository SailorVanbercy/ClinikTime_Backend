using ClinikTime.service;
using ClinikTime.service.Auth;
using ClinikTime.service.jwt;
using ClinikTime.utils;
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
// Configuration JWT
// ======================
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);
var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt!.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwt.SecretKey)
        )
    };
});
builder.Services.AddAuthorization();


// ======================
// Dependency Injection
// ======================
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();