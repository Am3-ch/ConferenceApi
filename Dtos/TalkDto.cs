namespace Conference.Dtos;

public record class TalkDto
(
    int Id,
    string Title,
    int SpeakerId,
    string time
);
