using System;

namespace Conference.Dtos;

public record UpdateTalkDto
(
    string Title,
    int SpeakerId,
    string time
);
