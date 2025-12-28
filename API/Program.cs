using ClinikTime.service;
using ClinikTime.service.Auth;
using ClinikTime.service.Disponibilite;
using ClinikTime.service.Email;
using ClinikTime.service.FichePatient;
using ClinikTime.service.jwt;
using ClinikTime.service.Medecin;
using ClinikTime.service.RendezVous;
// 1. AJOUT DES IMPORTS MANQUANTS
using ClinikTime.service.PasswordReset;
using ClinikTime.utils;
using ClinikTime.utils.PasswordHasher;
using Infrastructure.Data;
using Infrastructure.user.EF;
using Infrastructure.user.EF.Disponibilite;
using Infrastructure.user.EF.FichePatient;
using Infrastructure.user.EF.Medecin;
using Infrastructure.user.EF.RendezVous;
// 2. AJOUT DU REPOSITORY MANQUANT
using Infrastructure.user.EF.PasswordReset;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ======================
// Services - Configuration
// ======================

// Politique CORS (Indispensable pour Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // L'URL de ton Frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers();

// Swagger
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

if (jwt == null)
{
    throw new InvalidOperationException("La section 'JwtSettings' est manquante dans appsettings.json");
}

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("jwt"))
            {
                context.Token = context.Request.Cookies["jwt"];
            }
            return Task.CompletedTask;
        }
    };
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwt.SecretKey)
        )
    };
});
builder.Services.AddAuthorization();


// ======================
// Dependency Injection (Services & Repositories)
// ======================

// Repositories
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IMedecinRepository, MedecinRepository>();
builder.Services.AddScoped<IRendezVousRepository, RendezVousRepository>();
builder.Services.AddScoped<IFichePatientRepository, FichePatientrepository>();
builder.Services.AddScoped<IDisponibiliteMedecinRepository, DisponibiliteMedecinRepository>();
// -> AJOUT ICI :
builder.Services.AddScoped<IPasswordRepository, PasswordRepository>();

// Services Métier
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MedecinService>();
builder.Services.AddScoped<RendezVousService>();
builder.Services.AddScoped<FichePatientService>();
builder.Services.AddScoped<DisponibiliteMedecinService>();
// -> AJOUT ICI :
builder.Services.AddScoped<PasswordResetService>();

// Outils & Auth
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();


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

// Le HTTPS Redirection peut parfois causer des warnings en dev local si le port HTTPS n'est pas configuré, 
// mais ce n'est pas bloquant.
// app.UseHttpsRedirection();

app.UseRouting();
// CORS doit être AVANT Auth
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();