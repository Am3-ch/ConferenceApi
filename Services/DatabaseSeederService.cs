using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ConferenceApi.Services;

/// <summary>
/// Service responsible for seeding initial data into the database.
/// Only seeds data if the database is empty to prevent duplicates.
/// </summary>
public class DatabaseSeederService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseSeederService> _logger;
    private readonly IWebHostEnvironment _environment;

    public DatabaseSeederService(
        AppDbContext context,
        ILogger<DatabaseSeederService> logger,
        IWebHostEnvironment environment)
    {
        _context = context;
        _logger = logger;
        _environment = environment;
    }

    public async Task SeedAsync()
    {
        // Skip seeding if data already exists
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Database already contains data. Skipping seed.");
            return;
        }

        _logger.LogInformation("Starting database seeding...");

        var seedData = await LoadSeedDataAsync();
        if (seedData is null)
        {
            _logger.LogWarning("No seed data found or failed to load.");
            return;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Seed Users
            var userMap = await SeedUsersAsync(seedData.Users);

            // Seed Speakers (requires users to exist first)
            var speakerMap = await SeedSpeakersAsync(seedData.Speakers, userMap);

            // Seed Talks (requires speakers to exist first)
            await SeedTalksAsync(seedData.Talks, speakerMap);

            await transaction.CommitAsync();
            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error occurred during database seeding. Transaction rolled back.");
            throw;
        }
    }

    private async Task<SeedDataModel?> LoadSeedDataAsync()
    {
        var seedFilePath = Path.Combine(_environment.ContentRootPath, "Data", "SeedData.json");

        if (!File.Exists(seedFilePath))
        {
            _logger.LogWarning("Seed data file not found at: {Path}", seedFilePath);
            return null;
        }

        var jsonContent = await File.ReadAllTextAsync(seedFilePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<SeedDataModel>(jsonContent, options);
    }

    private async Task<Dictionary<string, int>> SeedUsersAsync(List<SeedUser> users)
    {
        var userMap = new Dictionary<string, int>();

        foreach (var seedUser in users)
        {
            var user = new User
            {
                Username = seedUser.Username,
                Email = seedUser.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedUser.Password),
                FirstName = seedUser.FirstName,
                LastName = seedUser.LastName,
                Role = seedUser.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userMap[seedUser.Username] = user.Id;
            _logger.LogDebug("Seeded user: {Username} with role: {Role}", user.Username, user.Role);
        }

        _logger.LogInformation("Seeded {Count} users.", users.Count);
        return userMap;
    }

    private async Task<Dictionary<string, int>> SeedSpeakersAsync(
        List<SeedSpeaker> speakers,
        Dictionary<string, int> userMap)
    {
        var speakerMap = new Dictionary<string, int>();

        foreach (var seedSpeaker in speakers)
        {
            if (!userMap.TryGetValue(seedSpeaker.Username, out var userId))
            {
                _logger.LogWarning("User not found for speaker: {Username}. Skipping.", seedSpeaker.Username);
                continue;
            }

            var speaker = new Speaker
            {
                UserId = userId,
                FullName = seedSpeaker.FullName,
                Bio = seedSpeaker.Bio,
                Company = seedSpeaker.Company,
                JobTitle = seedSpeaker.JobTitle,
                TwitterHandle = seedSpeaker.TwitterHandle,
                LinkedInUrl = seedSpeaker.LinkedInUrl,
                WebsiteUrl = seedSpeaker.WebsiteUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Speakers.Add(speaker);
            await _context.SaveChangesAsync();

            speakerMap[seedSpeaker.Username] = speaker.Id;
            _logger.LogDebug("Seeded speaker: {FullName}", speaker.FullName);
        }

        _logger.LogInformation("Seeded {Count} speakers.", speakers.Count);
        return speakerMap;
    }

    private async Task SeedTalksAsync(
        List<SeedTalk> talks,
        Dictionary<string, int> speakerMap)
    {
        // Base date for scheduling talks (start from tomorrow)
        var baseDate = DateTime.UtcNow.Date.AddDays(1).AddHours(9); // 9 AM tomorrow

        foreach (var seedTalk in talks)
        {
            if (!speakerMap.TryGetValue(seedTalk.SpeakerUsername, out var speakerId))
            {
                _logger.LogWarning("Speaker not found for talk: {Title}. Skipping.", seedTalk.Title);
                continue;
            }

            var talk = new Talk
            {
                SpeakerId = speakerId,
                Title = seedTalk.Title,
                Description = seedTalk.Description,
                ScheduledAt = baseDate.AddDays(seedTalk.ScheduledAtOffset),
                DurationMinutes = seedTalk.DurationMinutes,
                Room = seedTalk.Room,
                Level = seedTalk.Level,
                Category = seedTalk.Category,
                MaxAttendees = seedTalk.MaxAttendees,
                CurrentAttendees = 0,
                Status = "Scheduled",
                CreatedAt = DateTime.UtcNow
            };

            _context.Talks.Add(talk);
            _logger.LogDebug("Seeded talk: {Title}", talk.Title);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} talks.", talks.Count);
    }
}

#region Seed Data Models

/// <summary>
/// Root model for deserializing the seed data JSON file.
/// </summary>
public class SeedDataModel
{
    public List<SeedUser> Users { get; set; } = [];
    public List<SeedSpeaker> Speakers { get; set; } = [];
    public List<SeedTalk> Talks { get; set; } = [];
}

public class SeedUser
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; } = "Attendee";
}

public class SeedSpeaker
{
    public required string Username { get; set; }
    public required string FullName { get; set; }
    public required string Bio { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public string? TwitterHandle { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}

public class SeedTalk
{
    public required string SpeakerUsername { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int ScheduledAtOffset { get; set; } = 0;
    public int DurationMinutes { get; set; } = 60;
    public string? Room { get; set; }
    public string Level { get; set; } = "Intermediate";
    public string? Category { get; set; }
    public int MaxAttendees { get; set; } = 100;
}

#endregion

