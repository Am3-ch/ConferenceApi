using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Authorize(Roles = "Admin, Speaker")]
public class SpeakersController : ControllerBase
{
    private readonly AppDbContext _context;

    public SpeakersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/speakers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpeakerResponseDto>>> GetSpeakers()
    {
        var speakers = await _context.Speakers
            .Include(s => s.Talks)
            .Select(s => new SpeakerResponseDto
            {
                Id = s.Id,
                FullName = s.FullName,
                Bio = s.Bio,
                Company = s.Company,
                JobTitle = s.JobTitle,
                ProfileImageUrl = s.ProfileImageUrl,
                TwitterHandle = s.TwitterHandle,
                LinkedInUrl = s.LinkedInUrl,
                WebsiteUrl = s.WebsiteUrl,
                TotalTalks = s.Talks.Count
            })
            .ToListAsync();

        return Ok(speakers);
    }

    // GET: api/speakers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SpeakerResponseDto>> GetSpeaker(int id)
    {
        var speaker = await _context.Speakers
            .Include(s => s.Talks)
            .Where(s => s.Id == id)
            .Select(s => new SpeakerResponseDto
            {
                Id = s.Id,
                FullName = s.FullName,
                Bio = s.Bio,
                Company = s.Company,
                JobTitle = s.JobTitle,
                ProfileImageUrl = s.ProfileImageUrl,
                TwitterHandle = s.TwitterHandle,
                LinkedInUrl = s.LinkedInUrl,
                WebsiteUrl = s.WebsiteUrl,
                TotalTalks = s.Talks.Count
            })
            .FirstOrDefaultAsync();

        if (speaker == null)
            return NotFound(new { message = "Speaker not found" });

        return Ok(speaker);
    }

    // POST: api/auth/speakers
    [HttpPost]
    public async Task<ActionResult<SpeakerResponseDto>> CreateSpeaker([FromBody] CreateSpeakerDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        // Check if user already has a speaker profile
        if (await _context.Speakers.AnyAsync(s => s.UserId == userId))
            return BadRequest(new { message = "Speaker profile already exists" });

        var speaker = new Speaker
        {
            UserId = userId,
            FullName = dto.FullName,
            Bio = dto.Bio,
            Company = dto.Company,
            JobTitle = dto.JobTitle,
            ProfileImageUrl = dto.ProfileImageUrl,
            TwitterHandle = dto.TwitterHandle,
            LinkedInUrl = dto.LinkedInUrl,
            WebsiteUrl = dto.WebsiteUrl
        };

        _context.Speakers.Add(speaker);
        await _context.SaveChangesAsync();

        // Update user role to Speaker
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Role = "Speaker";
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetSpeaker), new { id = speaker.Id }, new SpeakerResponseDto
        {
            Id = speaker.Id,
            FullName = speaker.FullName,
            Bio = speaker.Bio,
            Company = speaker.Company,
            JobTitle = speaker.JobTitle,
            ProfileImageUrl = speaker.ProfileImageUrl,
            TwitterHandle = speaker.TwitterHandle,
            LinkedInUrl = speaker.LinkedInUrl,
            WebsiteUrl = speaker.WebsiteUrl,
            TotalTalks = 0
        });
    }

    // PUT: api/speakers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpeaker(int id, [FromBody] CreateSpeakerDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var speaker = await _context.Speakers.FindAsync(id);
        if (speaker == null)
            return NotFound(new { message = "Speaker not found" });

        // Check if user owns this speaker profile
        if (speaker.UserId != userId)
            return Forbid();

        speaker.FullName = dto.FullName;
        speaker.Bio = dto.Bio;
        speaker.Company = dto.Company;
        speaker.JobTitle = dto.JobTitle;
        speaker.ProfileImageUrl = dto.ProfileImageUrl;
        speaker.TwitterHandle = dto.TwitterHandle;
        speaker.LinkedInUrl = dto.LinkedInUrl;
        speaker.WebsiteUrl = dto.WebsiteUrl;
        speaker.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Speaker updated successfully" });
    }

    // GET: api/speakers/5/talks
    [HttpGet("{id}/talks")]
    public async Task<ActionResult<IEnumerable<TalkResponseDto>>> GetSpeakerTalks(int id)
    {
        var speaker = await _context.Speakers.FindAsync(id);
        if (speaker == null)
            return NotFound(new { message = "Speaker not found" });

        var talks = await _context.Talks
            .Include(t => t.Speaker)
            .Where(t => t.SpeakerId == id)
            .Select(t => new TalkResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                ScheduledAt = t.ScheduledAt,
                DurationMinutes = t.DurationMinutes,
                Room = t.Room,
                Level = t.Level,
                Category = t.Category,
                MaxAttendees = t.MaxAttendees,
                CurrentAttendees = t.CurrentAttendees,
                Status = t.Status,
                Speaker = new SpeakerResponseDto
                {
                    Id = t.Speaker.Id,
                    FullName = t.Speaker.FullName,
                    Bio = t.Speaker.Bio,
                    Company = t.Speaker.Company,
                    JobTitle = t.Speaker.JobTitle,
                    ProfileImageUrl = t.Speaker.ProfileImageUrl,
                    TwitterHandle = t.Speaker.TwitterHandle,
                    LinkedInUrl = t.Speaker.LinkedInUrl,
                    WebsiteUrl = t.Speaker.WebsiteUrl,
                    TotalTalks = 0
                },
                IsUserRegistered = false
            })
            .ToListAsync();

        return Ok(talks);
    }
}
