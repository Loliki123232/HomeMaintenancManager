using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HomeMaintenanceManager.Core.Enums;
using HomeMaintenanceManager.Core.Interfaces;
using HomeMaintenanceManager.Core.Models;
using HomeMaintenanceManager.Data.Context;

namespace HomeMaintenanceManager.Data.Repositories;

public class MaintenanceTaskRepository : IMaintenanceTaskRepository
{
    private readonly ApplicationDbContext _context;

    public MaintenanceTaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MaintenanceTask?> GetByIdAsync(int id)
    {
        return await _context.MaintenanceTasks
            .Include(t => t.StatusHistory)
            .Include(t => t.Checklist)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<MaintenanceTask>> GetAllAsync()
    {
        return await _context.MaintenanceTasks
            .Include(t => t.StatusHistory)
            .Include(t => t.Checklist)
            .Include(t => t.Attachments)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceTask>> GetOverdueTasksAsync()
    {
        return await _context.MaintenanceTasks
            .Include(t => t.StatusHistory)
            .Where(t => t.PlannedDate < DateTime.Now && t.Status != Status.Completed)
            .OrderBy(t => t.PlannedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceTask>> GetTasksByPriorityAsync(Priority priority)
    {
        return await _context.MaintenanceTasks
            .Include(t => t.StatusHistory)
            .Where(t => t.Priority == priority)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceTask>> GetTasksByCategoryAsync(TaskCategory category)
    {
        return await _context.MaintenanceTasks
            .Include(t => t.StatusHistory)
            .Where(t => t.Category == category)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();
    }

    public async Task<MaintenanceTask> AddAsync(MaintenanceTask task)
    {
        _context.MaintenanceTasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(MaintenanceTask task)
    {
        _context.MaintenanceTasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await GetByIdAsync(id);
        if (task != null)
        {
            _context.MaintenanceTasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}