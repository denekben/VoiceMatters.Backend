﻿using MediatR;

using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Petitions.Queries
{
    public sealed record GetPetition(Guid Id, bool AllowBlocked = false) : IRequest<PetitionDto?>;
}
