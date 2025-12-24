namespace HomeMaintenanceManager.Core.Models;

public class ChecklistItem
{
    public int Id { get; set; }
    public int MaintenanceTaskId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int Order { get; set; }
}