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

            position.IsActive = false;
            await _unitOfWork.CommitTransactionAsync(async () =>
            {
                await _positionRepository.UpdateAsync(position);
            }, cancellationToken);
        }

        public async Task<(IEnumerable<PositionResponse> items, int total)> GetAllPositionsAsync(bool? isActive, CancellationToken cancellationToken)
        {
            var query = (await _positionRepository.GetAllAsync(cancellationToken))
                .Cast<Position>();

            // Filter by isActive only if parameter is provided
            if (isActive.HasValue)
                query = query.Where(p => p.IsActive == isActive.Value);

            var positions = query.ToList();
            var total = positions.Count;

            var items = positions.Select(position =>
                new PositionResponse(
                    position.Id,
                    position.Title ?? string.Empty,
                    position.IsActive
                )
            );

            return (items, total);
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
