using System.ComponentModel.DataAnnotations;

namespace LevelUp.API.DTOs.ModuleItems
{
  public class ReorderModuleItemsRequest
  {
    [Required]
    public List<ItemOrderDto> ItemOrders { get; set; } = null!;
  }

  public class ItemOrderDto
  {
    [Required]
    public Guid ItemId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int NewOrderIndex { get; set; }
  }
}
