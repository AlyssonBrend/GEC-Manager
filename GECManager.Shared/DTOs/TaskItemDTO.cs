using GECManager.Shared.Models;

namespace GECManager.Shared.DTOs;

public class TaskItemDTO
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public int ProjectId { get; set; }
    public int? AssignedUserId { get; set; }
    public DateTime? DueDate { get; set; }
}
