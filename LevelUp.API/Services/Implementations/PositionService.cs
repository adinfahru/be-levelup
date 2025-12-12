using LevelUp.API.Entity;
using LevelUpAPI.Services.Interfaces;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.DTOs.Positions;

namespace LevelUpAPI.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IUnitOfWork _unitOfWork;


        public PositionService(IPositionRepository positionRepository, IUnitOfWork unitOfWork)
        {
            _positionRepository = positionRepository;  
            _unitOfWork = unitOfWork;
        }

        public async Task CreatePositionAsync(PositionRequest request, CancellationToken cancellationToken)
        {
            var position = new Position
            {
                Id = Guid.NewGuid(),
                Title = request.Title ?? string.Empty,
                IsActive = request.IsActive ?? true
            };

            await _unitOfWork.CommitTransactionAsync(async () =>
            {
                await _positionRepository.CreateAsync(position, cancellationToken);
            }, cancellationToken);
        }

        public async Task DeletePositionAsync(Guid id, CancellationToken cancellationToken)
        {
            var position = await _positionRepository.GetByIdAsync(id, cancellationToken);
            if (position is null)
                throw new NullReferenceException("Position Id not found");

            await _unitOfWork.CommitTransactionAsync(async () =>
            {
                await _positionRepository.DeleteAsync(position);
            }, cancellationToken);
        }

        public async Task<IEnumerable<PositionResponse>> GetAllPositionsAsync(CancellationToken cancellationToken)
        {
            // var positions = await _positionRepository.GetAllAsync(cancellationToken);
            var positions = (await _positionRepository.GetAllAsync(cancellationToken))
                .Cast<Position>()
                .ToList();


            if (!positions.Any())
                throw new NullReferenceException("No Positions Found");

            return positions.Select(position =>
            new PositionResponse(
                position.Id,
                position.Title ?? string.Empty,
                position.IsActive
            )
        );

        }

        public async Task<PositionResponse?> GetPositionByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var position = await _positionRepository.GetByIdAsync(id, cancellationToken);
            if (position is null)
                throw new NullReferenceException("Position Id not found");

            return new PositionResponse(
                position.Id,
                position.Title ?? string.Empty,
                position.IsActive
            );
        }

        public async Task UpdatePositionAsync(Guid id, PositionRequest request, CancellationToken cancellationToken)
        {
            var position = await _positionRepository.GetByIdAsync(id, cancellationToken);
            if (position is null)
                throw new NullReferenceException("Position Id not found");

            if (request.Title is not null)
                position.Title = request.Title;

            if (request.IsActive.HasValue)
                position.IsActive = request.IsActive.Value;

            await _unitOfWork.CommitTransactionAsync(async () =>
            {
                await _positionRepository.UpdateAsync(position);
            }, cancellationToken);
        }
    }
}
