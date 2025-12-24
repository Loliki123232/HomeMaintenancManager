using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HomeMaintenanceManager.Core.Interfaces;
using HomeMaintenanceManager.Core.Services;
using HomeMaintenanceManager.Data.Context;
using HomeMaintenanceManager.Data.Repositories;
using HomeMaintenanceManager.WPF.ViewModels;

namespace HomeMaintenanceManager.WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Упрощенная инициализация без Dependency Injection
        var dbContext = new ApplicationDbContext();
        var repository = new MaintenanceTaskRepository(dbContext);
        var taskService = new MaintenanceTaskService(repository);
        var mainViewModel = new MainViewModel(taskService);

        // Инициализация базы данных
        dbContext.Database.EnsureCreated();

        var mainWindow = new MainWindow();
        mainWindow.DataContext = mainViewModel;
        mainWindow.Show();

        base.OnStartup(e);
    }
}