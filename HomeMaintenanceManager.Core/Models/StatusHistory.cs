using System;
using HomeMaintenanceManager.Core.Enums;

namespace HomeMaintenanceManager.Core.Models;

public class StatusHistory
{
    public int Id { get; set; }
    public int MaintenanceTaskId { get; set; }
    public Status OldStatus { get; set; }
    public Status NewStatus { get; set; }
    public DateTime ChangeDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
}