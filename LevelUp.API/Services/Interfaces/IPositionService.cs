using LevelUp.API.DTOs.Positions;

namespace LevelUpAPI.Services.Interfaces
{
    public interface IPositionService
    {
         Task CreatePositionAsync(PositionRequest request, CancellationToken cancellationToken);
        Task UpdatePositionAsync(Guid id, PositionRequest request, CancellationToken cancellationToken);
        Task DeletePositionAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<PositionResponse>> GetAllPositionsAsync(CancellationToken cancellationToken);
        Task<PositionResponse?> GetPositionByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
