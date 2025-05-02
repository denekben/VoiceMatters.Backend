using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Application.UseCases.Petitions.Commands.Handlers
{
    public sealed class SignPetitionHandler : IRequestHandler<SignPetition>
    {
        private readonly IAppUserPetitionRepository _userPetitionRepository;
        private readonly IPetitionRepository _petitionRepository;
        private readonly ILogger<SignPetitionHandler> _logger;
        private readonly IHttpContextService _contextService;
        private readonly IStatisticRepository _statisticRepository;
        private readonly INotificationService _notifications;
        private readonly IRepository _repository;

        public SignPetitionHandler(IAppUserPetitionRepository userPetitionRepository, ILogger<SignPetitionHandler> logger
            , IHttpContextService contextService, IPetitionRepository petitionRepository, IStatisticRepository statisticRepository
            , INotificationService notifications, IRepository repository)
        {
            _userPetitionRepository = userPetitionRepository;
            _logger = logger;
            _contextService = contextService;
            _petitionRepository = petitionRepository;
            _statisticRepository = statisticRepository;
            _notifications = notifications;
            _repository = repository;
        }

        public async Task Handle(SignPetition command, CancellationToken cancellationToken)
        {
            var petition = await _petitionRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find petition {command.Id}");

            petition.SignQuantityPerDay += 1;
            petition.SignQuantity += 1;

            var signerId = _contextService.GetCurrentUserId();

            var petitionSigner = await _userPetitionRepository.GetAsync(petition.Id, signerId);

            if (petitionSigner != null)
                throw new BadRequestException("Petition already signed");

            var sign = AppUserSignedPetition.Create(signerId, petition.Id)
                ?? throw new BadRequestException("Cannot sign petition");

            var stats = await _statisticRepository.GetAsync();
            if (stats != null)
            {
                stats.Update(StatParameter.SignsQuantity);
                await _notifications.PetitionSigned();
            }

            await _userPetitionRepository.AddAsync(sign);

            await _repository.SaveChangesAsync();

            _logger.LogInformation("Petition signed");
        }
    }
}
