using LevelUp.API.DTOs.Modules;

namespace LevelUp.API.Services.Interfaces;

public interface IModuleService
{
  Task<(List<ModuleResponse> items, int total)> GetAllAsync(int page, int limit, bool? isActive, Guid? createdBy);
  Task<ModuleDetailResponse?> GetByIdAsync(Guid id);
  Task<ModuleResponse> CreateAsync(CreateModuleRequest request, Guid managerId);
  Task<ModuleResponse?> UpdateAsync(Guid id, UpdateModuleRequest request, Guid managerId);
}
