using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TalksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TalksController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/talks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TalkResponseDto>>> GetTalks(
        [FromQuery] string? category = null,
        [FromQuery] string? level = null,
        [FromQuery] DateTime? date = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdClaim != null ? int.Parse(userIdClaim) : (int?)null;

        var query = _context.Talks
            .Include(t => t.Speaker)
            .Include(t => t.Registrations)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category == category);

        if (!string.IsNullOrEmpty(level))
            query = query.Where(t => t.Level == level);

        if (date.HasValue)
            query = query.Where(t => t.ScheduledAt.Date == date.Value.Date);

        var talks = await query
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
                IsUserRegistered = userId.HasValue && t.Registrations.Any(r => r.UserId == userId.Value)
            })
            .OrderBy(t => t.ScheduledAt)
            .ToListAsync();

        return Ok(talks);
    }

    // GET: api/talks/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TalkResponseDto>> GetTalk(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdClaim != null ? int.Parse(userIdClaim) : (int?)null;

        var talk = await _context.Talks
            .Include(t => t.Speaker)
            .Include(t => t.Registrations)
            .Where(t => t.Id == id)
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
                IsUserRegistered = userId.HasValue && t.Registrations.Any(r => r.UserId == userId.Value)
            })
            .FirstOrDefaultAsync();

        if (talk == null)
            return NotFound(new { message = "Talk not found" });

        return Ok(talk);
    }

    // POST: api/talks
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TalkResponseDto>> CreateTalk([FromBody] CreateTalkDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        // Get speaker profile for this user
        var speaker = await _context.Speakers.FirstOrDefaultAsync(s => s.UserId == userId);
        if (speaker == null)
            return BadRequest(new { message = "You must create a speaker profile first" });

        var talk = new Talk
        {
            Title = dto.Title,
            Description = dto.Description,
            SpeakerId = speaker.Id,
            ScheduledAt = dto.ScheduledAt,
            DurationMinutes = dto.DurationMinutes,
            Room = dto.Room,
            Level = dto.Level,
            Category = dto.Category,
            MaxAttendees = dto.MaxAttendees
        };

        _context.Talks.Add(talk);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTalk), new { id = talk.Id }, new { message = "Talk created successfully", talkId = talk.Id });
    }

    // PUT: api/talks/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTalk(int id, [FromBody] UpdateTalkDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var talk = await _context.Talks
            .Include(t => t.Speaker)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (talk == null)
            return NotFound(new { message = "Talk not found" });

        // Check if user owns this talk
        if (talk.Speaker.UserId != userId)
            return Forbid();

        if (dto.Title != null) talk.Title = dto.Title;
        if (dto.Description != null) talk.Description = dto.Description;
        if (dto.ScheduledAt.HasValue) talk.ScheduledAt = dto.ScheduledAt.Value;
        if (dto.DurationMinutes.HasValue) talk.DurationMinutes = dto.DurationMinutes.Value;
        if (dto.Room != null) talk.Room = dto.Room;
        if (dto.Level != null) talk.Level = dto.Level;
        if (dto.Category != null) talk.Category = dto.Category;
        if (dto.MaxAttendees.HasValue) talk.MaxAttendees = dto.MaxAttendees.Value;
        if (dto.Status != null) talk.Status = dto.Status;
        
        talk.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Talk updated successfully" });
    }

    // DELETE: api/talks/5
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTalk(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var talk = await _context.Talks
            .Include(t => t.Speaker)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (talk == null)
            return NotFound(new { message = "Talk not found" });

        // Check if user owns this talk
        if (talk.Speaker.UserId != userId)
            return Forbid();

        _context.Talks.Remove(talk);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Talk deleted successfully" });
    }

    // POST: api/talks/5/register
    [Authorize]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> RegisterForTalk(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var talk = await _context.Talks
            .Include(t => t.Registrations)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (talk == null)
            return NotFound(new { message = "Talk not found" });

        // Check if already registered
        if (talk.Registrations.Any(r => r.UserId == userId))
            return BadRequest(new { message = "Already registered for this talk" });

        // Check if talk is full
        if (talk.CurrentAttendees >= talk.MaxAttendees)
            return BadRequest(new { message = "This talk is full" });

        var registration = new TalkRegistration
        {
            TalkId = id,
            UserId = userId
        };

        _context.TalkRegistrations.Add(registration);
        talk.CurrentAttendees++;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Successfully registered for talk" });
    }

    // DELETE: api/talks/5/register
    [Authorize]
    [HttpDelete("{id}/register")]
    public async Task<IActionResult> UnregisterFromTalk(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var registration = await _context.TalkRegistrations
            .FirstOrDefaultAsync(r => r.TalkId == id && r.UserId == userId);

        if (registration == null)
            return NotFound(new { message = "Registration not found" });

        var talk = await _context.Talks.FindAsync(id);
        if (talk != null)
        {
            talk.CurrentAttendees--;
        }

        _context.TalkRegistrations.Remove(registration);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Successfully unregistered from talk" });
    }

    // GET: api/talks/my-registrations
    [Authorize]
    [HttpGet("my-registrations")]
    public async Task<ActionResult<IEnumerable<TalkResponseDto>>> GetMyRegistrations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var talks = await _context.TalkRegistrations
            .Include(r => r.Talk)
                .ThenInclude(t => t.Speaker)
            .Where(r => r.UserId == userId)
            .Select(r => new TalkResponseDto
            {
                Id = r.Talk.Id,
                Title = r.Talk.Title,
                Description = r.Talk.Description,
                ScheduledAt = r.Talk.ScheduledAt,
                DurationMinutes = r.Talk.DurationMinutes,
                Room = r.Talk.Room,
                Level = r.Talk.Level,
                Category = r.Talk.Category,
                MaxAttendees = r.Talk.MaxAttendees,
                CurrentAttendees = r.Talk.CurrentAttendees,
                Status = r.Talk.Status,
                Speaker = new SpeakerResponseDto
                {
                    Id = r.Talk.Speaker.Id,
                    FullName = r.Talk.Speaker.FullName,
                    Bio = r.Talk.Speaker.Bio,
                    Company = r.Talk.Speaker.Company,
                    JobTitle = r.Talk.Speaker.JobTitle,
                    ProfileImageUrl = r.Talk.Speaker.ProfileImageUrl,
                    TwitterHandle = r.Talk.Speaker.TwitterHandle,
                    LinkedInUrl = r.Talk.Speaker.LinkedInUrl,
                    WebsiteUrl = r.Talk.Speaker.WebsiteUrl,
                    TotalTalks = 0
                },
                IsUserRegistered = true
            })
            .ToListAsync();

        return Ok(talks);
    }
}