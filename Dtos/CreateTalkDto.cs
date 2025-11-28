using System;

namespace Conference.Dtos;

public record CreateTalkDto
(
    string Title,
    int SpeakerId,
    string time
);

