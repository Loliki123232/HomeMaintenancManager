using Microsoft.EntityFrameworkCore;
using HomeMaintenanceManager.Core.Models;

namespace HomeMaintenanceManager.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // ДОБАВЬТЕ ЭТОТ КОНСТРУКТОР
    public ApplicationDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=maintenance.db");
        }
    }

    public DbSet<MaintenanceTask> MaintenanceTasks => Set<MaintenanceTask>();
    public DbSet<StatusHistory> StatusHistory => Set<StatusHistory>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
}