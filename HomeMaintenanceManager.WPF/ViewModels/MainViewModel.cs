using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HomeMaintenanceManager.Core.Enums;
using HomeMaintenanceManager.Core.Interfaces;
using HomeMaintenanceManager.Core.Models;
using HomeMaintenanceManager.WPF;

namespace HomeMaintenanceManager.WPF.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly IMaintenanceTaskService _taskService;
    private MaintenanceTask? _selectedTask;
    private string _searchText = string.Empty;

    public ObservableCollection<MaintenanceTask> Tasks => AllTasks;
    public ObservableCollection<MaintenanceTask> AllTasks { get; set; } = new();
    public ObservableCollection<MaintenanceTask> OverdueTasks { get; set; } = new();
    public ObservableCollection<MaintenanceTask> UpcomingTasks { get; set; } = new();
    public ObservableCollection<MaintenanceTask> PlannedTasks { get; set; } = new();
    public ObservableCollection<MaintenanceTask> InProgressTasks { get; set; } = new();
    public ObservableCollection<MaintenanceTask> CompletedTasks { get; set; } = new();

    public MaintenanceTask? SelectedTask
    {
        get => _selectedTask;
        set => SetProperty(ref _selectedTask, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public ICommand CreateTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand MarkCompletedCommand { get; }
    public ICommand MarkInProgressCommand { get; }
    public ICommand DuplicateTaskCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    public MainViewModel(IMaintenanceTaskService taskService)
    {
        _taskService = taskService;

        CreateTaskCommand = new RelayCommand(CreateTask);
        EditTaskCommand = new RelayCommand<MaintenanceTask>(EditTask, CanEditTask);
        DeleteTaskCommand = new RelayCommand<MaintenanceTask>(DeleteTask, CanDeleteTask);
        MarkCompletedCommand = new RelayCommand<MaintenanceTask>(MarkCompleted, CanMarkCompleted);
        MarkInProgressCommand = new RelayCommand<MaintenanceTask>(MarkInProgress, CanMarkInProgress);
        DuplicateTaskCommand = new RelayCommand<MaintenanceTask>(DuplicateTask);
        RefreshCommand = new RelayCommand(async () => await LoadTasksAsync());
        ClearFiltersCommand = new RelayCommand(ClearFilters);

        _ = LoadTasksAsync();
    }

    public async Task LoadTasks()
    {
        await LoadTasksAsync();
    }

    private async Task LoadTasksAsync()
    {
        try
        {
            var tasks = await _taskService.GetAllTasksAsync();

            AllTasks.Clear();
            foreach (var task in tasks)
            {
                AllTasks.Add(task);
            }

            await LoadOverdueTasksAsync();
            await LoadUpcomingTasksAsync();
            GroupTasksByStatus();
        }
        catch (Exception ex)
        {
            // Обработка ошибок загрузки
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки задач: {ex.Message}");
        }
    }

    private async Task LoadOverdueTasksAsync()
    {
        var overdue = await _taskService.GetOverdueTasksAsync();
        OverdueTasks.Clear();
        foreach (var task in overdue)
        {
            OverdueTasks.Add(task);
        }
    }

    private async Task LoadUpcomingTasksAsync()
    {
        var upcoming = await _taskService.GetUpcomingTasksAsync(7);
        UpcomingTasks.Clear();
        foreach (var task in upcoming)
        {
            UpcomingTasks.Add(task);
        }
    }

    private void GroupTasksByStatus()
    {
        PlannedTasks.Clear();
        InProgressTasks.Clear();
        CompletedTasks.Clear();

        foreach (var task in AllTasks)
        {
            switch (task.Status)
            {
                case Status.Planned:
                    PlannedTasks.Add(task);
                    break;
                case Status.InProgress:
                    InProgressTasks.Add(task);
                    break;
                case Status.Completed:
                    CompletedTasks.Add(task);
                    break;
            }
        }
    }

    private void ClearFilters()
    {
        SearchText = string.Empty;
    }

    private async void CreateTask()
    {
        var newTask = new MaintenanceTask
        {
            Title = "Новая задача обслуживания",
            Category = TaskCategory.Electrical,
            Priority = Priority.Medium,
            Status = Status.Planned,
            Frequency = Frequency.Once,
            PlannedDate = DateTime.Now.AddDays(7),
            EstimatedCost = 0,
            Executor = Executor.Myself,
            Description = "Описание задачи",
            Notes = "Заметки по выполнению",
            CreatedDate = DateTime.Now
        };

        await _taskService.CreateTaskAsync(newTask);
        await LoadTasksAsync();
    }

    private bool CanEditTask(MaintenanceTask? task) => task != null;

    private async void EditTask(MaintenanceTask? task)
    {
        if (task != null)
        {
            SelectedTask = task;
            task.Title = $"{task.Title} (отредактировано)";
            await _taskService.UpdateTaskAsync(task);
            await LoadTasksAsync();
        }
    }

    private bool CanDeleteTask(MaintenanceTask? task) => task != null;

    private async void DeleteTask(MaintenanceTask? task)
    {
        if (task != null)
        {
            await _taskService.DeleteTaskAsync(task.Id);
            await LoadTasksAsync();
        }
    }

    private bool CanMarkCompleted(MaintenanceTask? task) =>
        task != null && task.Status != Status.Completed;

    private async void MarkCompleted(MaintenanceTask? task)
    {
        if (task != null)
        {
            await _taskService.UpdateTaskStatusAsync(task.Id, Status.Completed);
            await LoadTasksAsync();
        }
    }

    private bool CanMarkInProgress(MaintenanceTask? task) =>
        task != null && task.Status == Status.Planned;

    private async void MarkInProgress(MaintenanceTask? task)
    {
        if (task != null)
        {
            await _taskService.UpdateTaskStatusAsync(task.Id, Status.InProgress);
            await LoadTasksAsync();
        }
    }

    private async void DuplicateTask(MaintenanceTask? task)
    {
        if (task != null)
        {
            var duplicatedTask = new MaintenanceTask
            {
                Title = $"{task.Title} (копия)",
                Category = task.Category,
                Priority = task.Priority,
                Status = Status.Planned,
                Frequency = task.Frequency,
                PlannedDate = DateTime.Now.AddDays(7),
                EstimatedCost = task.EstimatedCost,
                ActualCost = task.ActualCost,
                Executor = task.Executor,
                Description = task.Description,
                Notes = task.Notes,
                CreatedDate = DateTime.Now
            };

            await _taskService.CreateTaskAsync(duplicatedTask);
            await LoadTasksAsync();
        }
    }

    // Статистика
    public int TotalTasksCount => AllTasks.Count;
    public int CompletedTasksCount => AllTasks.Count(t => t.Status == Status.Completed);
    public int OverdueTasksCount => OverdueTasks.Count;
    public decimal TotalEstimatedCost => AllTasks.Sum(t => t.EstimatedCost);
    public decimal TotalActualCost => AllTasks.Where(t => t.ActualCost.HasValue).Sum(t => t.ActualCost!.Value);
}