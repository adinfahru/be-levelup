using LevelUp.API.DTOs.ModuleItems;
using LevelUp.API.DTOs.Modules;

namespace LevelUp.API.Services.Interfaces;

public interface IModuleService
{
  Task<(List<ModuleResponse> items, int total)> GetAllAsync(int page, int limit, bool? isActive, Guid? createdBy);
  Task<ModuleDetailResponse?> GetByIdAsync(Guid id);
  Task<ModuleResponse> CreateAsync(CreateModuleRequest request, Guid managerId);
  Task<ModuleResponse?> UpdateAsync(Guid id, UpdateModuleRequest request, Guid managerId);
  Task<List<ModuleEnrollmentUserResponse>> GetModuleEnrollmentsAsync(Guid moduleId);

  // Module Items Management
  Task<ModuleItemResponse> AddItemAsync(Guid moduleId, DTOs.Modules.CreateModuleItemRequest request, Guid creatorId);
  Task<ModuleItemResponse?> UpdateItemAsync(Guid moduleId, Guid itemId, UpdateModuleItemRequest request, Guid creatorId);
  Task<bool> DeleteItemAsync(Guid moduleId, Guid itemId, Guid creatorId);
  Task<List<ModuleItemResponse>> ReorderItemsAsync(Guid moduleId, ReorderModuleItemsRequest request, Guid creatorId);
}
