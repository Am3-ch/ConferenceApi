using Conference.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string GetSpeakerEndpointName = "Get Speaker";
const string GetTalkEndpointName = "Get talk";

List<SpeakerDto> speakers = [
    new (1, "John", "Doe"),
    new (2, "Jane", "Smith"),
];

List<TalkDto> talks =
[
    new (1, "Introduction to C#", 1, "10:00 AM"),
    new (2, "Building APIs with ASP.NET Core", 2, "11:00 AM")
];

// GET /speakers
app.MapGet("speakers", () => speakers);

// GET /spakers/1
app.MapGet("speakers/{id}", (int id) =>
{
    SpeakerDto? speaker = speakers.Find(speaker => speaker.Id == id);

    return speaker is null ? Results.NotFound() : Results.Ok(speaker);
})
.WithName(GetSpeakerEndpointName);

// GET /talks
app.MapGet("talks", () => talks);

// GET /talks/1
app.MapGet("talks/{id}", (int id) =>
{
    TalkDto? talk = talks.Find(talk => talk.Id == id);

    return talk is null ? Results.NotFound() : Results.Ok(talk);
})
.WithName(GetTalkEndpointName);

// POST /talks/
app.MapPost("talks", (CreateTalkDto newTalk) =>
{
    TalkDto talk = new(
      talks.Count + 1,
      newTalk.Title,
      newTalk.SpeakerId,
      newTalk.time
    );

    talks.Add(talk);

    return Results.CreatedAtRoute(GetTalkEndpointName, new { id = talk.Id }, talk);
});

// POST /speakers/
app.MapPost("speakers", (CreateSpeakerDto newSpeaker) =>
{
    SpeakerDto speaker = new(
      speakers.Count + 1,
      newSpeaker.FirstName,
      newSpeaker.LastName
    );

    speakers.Add(speaker);

    return Results.CreatedAtRoute(GetSpeakerEndpointName, new { id = speaker.Id }, speaker);
});

//PUT /talks
app.MapPut("talks/{id}", (int id, UpdateTalkDto updatedTalk) =>
{
   var index = talks.FindIndex(talks => talks.Id == id);

   if(index == -1)
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
});

//PUT /speakers
app.MapPut("speakers/{id}", (int id, UpdateSpeakerDto updatedSpeaker) =>
{
   var index = speakers.FindIndex(speakers => speakers.Id == id);

   if(index == -1)
   {
      return Results.NotFound();
   }

   speakers[index] = new SpeakerDto(
    id,
    updatedSpeaker.FirstName,
    updatedSpeaker.LastName
   );

   return Results.NoContent();
});

//DELETE /talks/1
app.MapDelete("talks/{id}", (int id) =>
{
   talks.RemoveAll(talkId => talkId.Id == id);

   return Results.NoContent();
}
);

//DELETE /speakers/1
app.MapDelete("speakers/{id}", (int id) =>
{
   talks.RemoveAll(speakerId => speakerId.Id == id);

   return Results.NoContent();
}
);


app.MapGet("/", () => "Welcome to our conference!!!");

app.Run();
