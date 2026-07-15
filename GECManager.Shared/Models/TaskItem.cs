using System.Text.Json.Serialization;

namespace GECManager.Shared.Models;

// Como string no JSON, para API e clientes concordarem ("Pending", não 0)
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskItemStatus { Pending, InProgress, Done }

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public int ProjectId { get; set; }
    public int? AssignedUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
}
