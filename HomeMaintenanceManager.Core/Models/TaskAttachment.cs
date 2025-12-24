using System;

namespace HomeMaintenanceManager.Core.Models;

public class TaskAttachment
{
    public int Id { get; set; }
    public int MaintenanceTaskId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; } = DateTime.Now;
}