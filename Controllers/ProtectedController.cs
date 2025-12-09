using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Conference.Dtos;

[ApiController]
[Route("api/[controller]")]
[Authorize]// Requires JWT token
public class ProtectedController : ControllerBase
{

    const string GetSpeakerEndpointName = "Get Speaker";
    const string GetTalkEndpointName = "Get talk";
    List<TalkDto> talks =
    [
        new (1, "Introduction to C#", 1, "10:00 AM"),
        new (2, "Building APIs with ASP.NET Core", 2, "11:00 AM")
    ];

    List<SpeakerDto> speakers = [
        new (1, "Humphrey", "Chama"),
        new (2, "Jane", "Phiri"),
    ];

    [HttpGet("data")]
    public IActionResult GetProtectedData()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new
        {
            message = "This is protected data",
            userId,
            username
        });
    }

    //GET speakers
    [HttpGet("speakers")]
    public IActionResult GetProtectedSpeakers()
    {
        return Ok(speakers);
    }

    //GET talks
    [HttpGet("talks")]
    public IActionResult GetProtectedtalks()
    {
        return Ok(talks);
    }

    //GET talks by ID 
    [HttpGet("talks/{id}", Name = GetTalkEndpointName)]
    public IResult GetProtectedtalksId(int id)
    {
        TalkDto? talk = talks.Find(talk => talk.Id == id);

        return talk is null ? Results.NotFound() : Results.Ok(talk);
    }

    //GET speakers by ID 
    [HttpGet("speakers/{id}", Name = GetSpeakerEndpointName)]
    public IResult GetProtectedSpeakersId(int id)
    {
        SpeakerDto? speaker = speakers.Find(speaker => speaker.Id == id);

        return speaker is null ? Results.NotFound() : Results.Ok(speaker);
    }

    //POST speaker (add speaker to the List)
    [HttpPost("speakers")]
    public async Task<IResult> CreateSpeaker([FromBody] CreateSpeakerDto newSpeaker)
    {
        SpeakerDto speaker = new(
       speakers.Count + 1,
       newSpeaker.FirstName,
       newSpeaker.LastName
     );

        speakers.Add(speaker);

        return Results.CreatedAtRoute(GetSpeakerEndpointName, new { id = speaker.Id }, speaker);

    }

    //POST talk (add talk to the List)
    [HttpPost("talks")]
    public async Task<IResult> CreateTalk([FromBody] CreateTalkDto newTalk)
    {
        TalkDto talk = new(
         talks.Count + 1,
         newTalk.Title,
         newTalk.SpeakerId,
         newTalk.time
         );

        talks.Add(talk);

        return Results.CreatedAtRoute(GetTalkEndpointName, new { id = talk.Id }, talk);
    }

    //PUT talk (update or edit a specific talk)
    [HttpPut("talks/{id}")]
    public async Task<IResult> UpdateTalk([FromRoute] int id, [FromBody] UpdateTalkDto updatedTalk)
    {
        var index = talks.FindIndex(talks => talks.Id == id);

        if (index == -1)
        {
            return Results.NotFound();
        }

        talks[index] = new TalkDto(
        id,
        updatedTalk.Title,
        updatedTalk.SpeakerId,
        updatedTalk.time
        );

        return Results.NoContent();
    }

    //PUT speaker
    [HttpPut("speakers/{id}")]
    public async Task<IResult> UpdateSpeaker([FromRoute] int id, [FromBody] UpdateSpeakerDto updatedSpeaker)
    {
        var index = speakers.FindIndex(speakers => speakers.Id == id);

        if (index == -1)
        {
            return Results.NotFound();
        }

        speakers[index] = new SpeakerDto(
        id,
        updatedSpeaker.FirstName,
        updatedSpeaker.LastName
        );

        return Results.NoContent();
    }

    //DELETE speaker
    [HttpDelete("talks/{id}")]
    public async Task<IResult> DeleteTalk([FromRoute] int id)
    {
        talks.RemoveAll(talkId => talkId.Id == id);

        return Results.NoContent();
    }

    //DELETE speaker
    [HttpDelete("speakers/{id}")]
    public async Task<IResult> DeleteSpeaker([FromRoute] int id)
    {
        talks.RemoveAll(speakerId => speakerId.Id == id);

        return Results.NoContent();
    }

}