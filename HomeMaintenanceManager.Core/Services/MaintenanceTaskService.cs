using HomeMaintenanceManager.Core.Enums;
using HomeMaintenanceManager.Core.Interfaces;
using HomeMaintenanceManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeMaintenanceManager.Core.Services;

public class MaintenanceTaskService : IMaintenanceTaskService
{
    private readonly IMaintenanceTaskRepository _repository;

    public MaintenanceTaskService(IMaintenanceTaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<MaintenanceTask?> GetTaskByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<MaintenanceTask>> GetAllTasksAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<MaintenanceTask>> GetOverdueTasksAsync()
    {
        return await _repository.GetOverdueTasksAsync();
    }

    public async Task<IEnumerable<MaintenanceTask>> GetTasksByPriorityAsync(Priority priority)
    {
        return await _repository.GetTasksByPriorityAsync(priority);
    }

    public async Task<IEnumerable<MaintenanceTask>> GetTasksByCategoryAsync(TaskCategory category)
    {
        return await _repository.GetTasksByCategoryAsync(category);
    }

    public async Task<MaintenanceTask> CreateTaskAsync(MaintenanceTask task)
    {
        // Добавляем начальную историю статусов
        task.StatusHistory.Add(new StatusHistory
        {
            OldStatus = Status.Planned,
            NewStatus = task.Status,
            ChangeDate = DateTime.Now,
            Notes = "Task created"
        });

        return await _repository.AddAsync(task);
    }

    public async Task UpdateTaskAsync(MaintenanceTask task)
    {
        await _repository.UpdateAsync(task);
    }

    public async Task UpdateTaskStatusAsync(int taskId, Status newStatus)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task != null)
        {
            // Добавляем запись в историю
            task.StatusHistory.Add(new StatusHistory
            {
                OldStatus = task.Status,
                NewStatus = newStatus,
                ChangeDate = DateTime.Now
            });

            // Обновляем статус
            task.Status = newStatus;

            // Если задача выполнена, устанавливаем фактическую дату
            if (newStatus == Status.Completed && task.ActualDate == null)
            {
                task.ActualDate = DateTime.Now;
            }

            await _repository.UpdateAsync(task);
        }
    }

    public async Task DeleteTaskAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public DateTime CalculateNextOccurrence(DateTime baseDate, Frequency frequency)
    {
        return frequency switch
        {
            Frequency.Monthly => baseDate.AddMonths(1),
            Frequency.Quarterly => baseDate.AddMonths(3),
            Frequency.Yearly => baseDate.AddYears(1),
            Frequency.Once => baseDate,
            _ => baseDate
        };
    }

    public async Task<IEnumerable<MaintenanceTask>> GetUpcomingTasksAsync(int daysAhead)
    {
        var allTasks = await _repository.GetAllAsync();
        var targetDate = DateTime.Now.AddDays(daysAhead);

        return allTasks.Where(t =>
            t.PlannedDate <= targetDate &&
            t.PlannedDate >= DateTime.Now &&
            t.Status != Status.Completed);
    }
}