using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using generar_ticket.Shared.Interfaces.ASP.Configuration;
using generar_ticket.area.Interfaces.REST.Transform;
using generar_ticket.Observaciones.Domain.Services;
using generar_ticket.Observaciones.Interfaces.REST.Transform;
using generar_ticket.ticket.Application.Internal.QueryServices;
using generar_ticket.ticket.Domain.Services;
using generar_ticket.Users.Application.Internal.CommandServices;
using generar_ticket.Users.Application.Internal.OutboundServices;
using generar_ticket.Users.Domain.Repositories;
using generar_ticket.Users.Domain.Services;
using generar_ticket.Users.Infrastructure.Persistence.EFC.Repositories;
using generar_ticket.Users.Infrastructure.Tokens.Configuration;
using generar_ticket.Users.Infrastructure.Tokens.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Set the HTTPS port

// Add services to the container.
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));

// Add Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}

// Configure Database Context and Logging Levels
builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        if (connectionString != null)
            if (builder.Environment.IsDevelopment())
                options.UseMySQL(connectionString)
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            else if (builder.Environment.IsProduction())
                options.UseMySQL(connectionString)
                    .LogTo(Console.WriteLine, LogLevel.Error)
                    .EnableDetailedErrors();
    });

// Configure JWT Authentication
var tokenSettings = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>();
var key = Encoding.ASCII.GetBytes(tokenSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "Generar Ticket - MUNICIPALIDAD DE JAEN API",
                Version = "v1",
                Description = "Generar Ticket API",
                TermsOfService = new Uri("https://example.com/tos"),
                Contact = new OpenApiContact
                {
                    Name = "Support",
                    Email = "support@example.com"
                },
                License = new OpenApiLicense
                {
                    Name = "Apache 2.0",
                    Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
                }
            });
        c.EnableAnnotations();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    });

// Configure Lowercase URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedAllPolicy",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register Area transformation services
builder.Services.AddScoped<AreaResourcesFromEntityAssembler>();
builder.Services.AddScoped<CreateAreaCommandFromResourcesAssembler>();

// Register HttpClient
builder.Services.AddHttpClient();

// Register PersonaService
builder.Services.AddScoped<PersonaService>();
builder.Services.AddScoped<ITicketQueryService, TicketQueryService>();

// Register user services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Comment transformation services
builder.Services.AddScoped<CommentResourceFromEntityAssembler>();
builder.Services.AddScoped<CreateCommentCommandFromResourceAssembler>();
builder.Services.AddScoped<ICommentCommandService, CommentCommandService>();

var app = builder.Build();

// Verify Database Objects are Created
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();

        Console.WriteLine("Ensuring database is created...");
        context.Database.EnsureCreated();
        Console.WriteLine("Database is ready.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred creating the DB: {ex.Message}\nStackTrace: {ex.StackTrace}");
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS Policy
app.UseCors("AllowedAllPolicy");

app.UseHttpsRedirection();

app.UseAuthentication(); // Add this line to enable authentication
app.UseAuthorization();

app.MapControllers();

Console.WriteLine("Starting the Application...");

app.Run();