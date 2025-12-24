using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeMaintenanceManager.Core.Enums;
using HomeMaintenanceManager.Core.Models;

namespace HomeMaintenanceManager.Core.Interfaces;

public interface IMaintenanceTaskService
{
    Task<MaintenanceTask?> GetTaskByIdAsync(int id);
    Task<IEnumerable<MaintenanceTask>> GetAllTasksAsync();
    Task<IEnumerable<MaintenanceTask>> GetOverdueTasksAsync();
    Task<IEnumerable<MaintenanceTask>> GetTasksByPriorityAsync(Priority priority);
    Task<IEnumerable<MaintenanceTask>> GetTasksByCategoryAsync(TaskCategory category);
    Task<MaintenanceTask> CreateTaskAsync(MaintenanceTask task);
    Task UpdateTaskAsync(MaintenanceTask task);
    Task UpdateTaskStatusAsync(int taskId, Status newStatus);
    Task DeleteTaskAsync(int id);
    DateTime CalculateNextOccurrence(DateTime baseDate, Frequency frequency);
    Task<IEnumerable<MaintenanceTask>> GetUpcomingTasksAsync(int daysAhead);
}