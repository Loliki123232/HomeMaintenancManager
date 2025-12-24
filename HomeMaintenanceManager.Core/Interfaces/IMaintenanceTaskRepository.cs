using System.Collections.Generic;
using System.Threading.Tasks;
using HomeMaintenanceManager.Core.Enums;
using HomeMaintenanceManager.Core.Models;

namespace HomeMaintenanceManager.Core.Interfaces;

public interface IMaintenanceTaskRepository
{
    Task<MaintenanceTask?> GetByIdAsync(int id);
    Task<IEnumerable<MaintenanceTask>> GetAllAsync();
    Task<IEnumerable<MaintenanceTask>> GetOverdueTasksAsync();
    Task<IEnumerable<MaintenanceTask>> GetTasksByPriorityAsync(Priority priority);
    Task<IEnumerable<MaintenanceTask>> GetTasksByCategoryAsync(TaskCategory category);
    Task<MaintenanceTask> AddAsync(MaintenanceTask task);
    Task UpdateAsync(MaintenanceTask task);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}