using System;
using System.Collections.Generic;
using HomeMaintenanceManager.Core.Enums;

namespace HomeMaintenanceManager.Core.Models;

public class MaintenanceTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskCategory Category { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public Frequency Frequency { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public Executor Executor { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public virtual List<StatusHistory> StatusHistory { get; set; } = new();
    public virtual List<ChecklistItem> Checklist { get; set; } = new();
    public virtual List<TaskAttachment> Attachments { get; set; } = new();
}