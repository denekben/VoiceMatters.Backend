using Microsoft.Extensions.Logging;
using Moq;
using VoiceMatters.Application.UseCases.Administration.Commands;
using VoiceMatters.Application.UseCases.Administration.Commands.Handlers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;


namespace VoiceMatters.Application.UseCases.Administration
{
    public class BlockPetitionHandlerTests
    {
        private readonly Mock<IPetitionRepository> _petitionRepository = new();
        private readonly Mock<ILogger<BlockPetitionHandler>> _logger = new();
        private readonly BlockPetitionHandler _handler;


        public BlockPetitionHandlerTests()
        {
            _handler = new BlockPetitionHandler(_petitionRepository.Object, _logger.Object);
        }

        [Fact]
        public async Task Handle_PetitionNotFound_ThrowsBadRequestException()
        {
            // Arange
            var command = new BlockPetition(Guid.NewGuid());
            _petitionRepository.Setup(x => x.GetAsync(command.Id)).ReturnsAsync((Petition?)null);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _petitionRepository.Verify(x => x.UpdateAsync(It.IsAny<Petition>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidationPassed_BlocksPetition()
        {
            // Arrange
            var petition = new Petition { Id = Guid.NewGuid(), IsBlocked = false };
            var command = new BlockPetition(petition.Id);

            _petitionRepository.Setup(x => x.GetAsync(command.Id))
                .ReturnsAsync(petition);

            _petitionRepository.Setup(x => x.UpdateAsync(It.IsAny<Petition>()))
                .Callback<Petition>(p => petition = p)
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(petition.IsBlocked);
            _petitionRepository.Verify(x => x.UpdateAsync(petition), Times.Once);
        }
    }
}
