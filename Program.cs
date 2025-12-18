using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authorization;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using ConferenceApi.Services;
using ConferenceApi.Middleware;

// Load environment variables from the .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Web", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<TokenCleanupService>();

//Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register JWT and Auth services
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DatabaseSeederService>();
// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("JWT Key is not configured. Set 'Jwt:Key' in appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(
                "https://conference-api-frontend-iumt.vercel.app/",
                "http://localhost:3000"
            ));
});




// Load environment variables from the .env file
Env.Load();



var app = builder.Build();

app.UseCors("AllowFrontend");
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/
app.UseSwagger();
app.UseSwaggerUI();

// CORS must come first to handle preflight OPTIONS requests
app.UseCors("Web");

// Global error handler
app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/", () => "Welcome to our conference!!!");

/*if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}*/

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// Applies migrations and seeds database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var startupLogger = services.GetRequiredService<ILogger<Program>>();

        // Connectivity check (helps distinguish DNS/network issues from migration issues)
        var canConnect = await context.Database.CanConnectAsync();
        startupLogger.LogInformation("Database connectivity check (CanConnect): {CanConnect}", canConnect);

        context.Database.Migrate();

        // Seed the database with initial data
        var seeder = services.GetRequiredService<DatabaseSeederService>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}

app.Run();
