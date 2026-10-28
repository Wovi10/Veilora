using System.Text;
using Veilora.Api.Middleware;
using Veilora.Api.Services;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services;
using Veilora.Application.Services.Interfaces;
using Veilora.Application.Settings;
using Veilora.Infrastructure.Email;
using Veilora.Infrastructure.Repositories;
using Veilora.Application.Validators;
using Veilora.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey == "REPLACE_VIA_USER_SECRETS")
    throw new InvalidOperationException(
        "JWT key is not configured. Run: dotnet user-secrets set \"Jwt:Key\" \"<secret>\" --project src/Veilora.Api");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IWorldRepository, WorldRepository>();
builder.Services.AddScoped<IEntityRepository, EntityRepository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IFamilyTreeRepository, FamilyTreeRepository>();
builder.Services.AddScoped<IRelationshipRepository, RelationshipRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWorldPermissionRepository, WorldPermissionRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IDateSuffixRepository, DateSuffixRepository>();
builder.Services.AddScoped<IReadingSessionRepository, ReadingSessionRepository>();
builder.Services.AddScoped<IReadingNoteRepository, ReadingNoteRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

// Services
builder.Services.AddScoped<IWorldService, WorldService>();
builder.Services.AddScoped<IEntityService, EntityService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IFamilyTreeService, FamilyTreeService>();
builder.Services.AddScoped<IRelationshipService, RelationshipService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IWorldPermissionService, WorldPermissionService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IDateSuffixService, DateSuffixService>();
builder.Services.AddScoped<IReadingSessionService, ReadingSessionService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Password reset / email configuration
builder.Services.AddSingleton(new PasswordResetSettings(
    ClientBaseUrl: builder.Configuration["App:ClientBaseUrl"] ?? "http://localhost:5173",
    TokenExpiryMinutes: int.Parse(builder.Configuration["PasswordReset:TokenExpiryMinutes"] ?? "60")));

var emailSettings = builder.Configuration.GetSection("Email");
builder.Services.AddSingleton(new EmailSettings(
    Host: emailSettings["Host"] ?? "localhost",
    Port: int.Parse(emailSettings["Port"] ?? "587"),
    FromAddress: emailSettings["FromAddress"] ?? "noreply@veilora.app",
    FromName: emailSettings["FromName"] ?? "Veilora",
    Username: emailSettings["Username"],
    Password: emailSettings["Password"],
    UseSsl: bool.Parse(emailSettings["UseSsl"] ?? "true")));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateWorldDtoValidator>();

// CORS - Allow React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
