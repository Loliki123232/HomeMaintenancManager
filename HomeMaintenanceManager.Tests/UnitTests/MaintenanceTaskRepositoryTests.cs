using HomeMaintenanceManager.Core.Enums;
using HomeMaintenanceManager.Core.Models;
using HomeMaintenanceManager.Data.Context;
using HomeMaintenanceManager.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeMaintenanceManager.Tests.UnitTests;

public class MaintenanceTaskRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly MaintenanceTaskRepository _repository;

    public MaintenanceTaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new MaintenanceTaskRepository(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var tasks = new List<MaintenanceTask>
        {
            new()
            {
                Title = "Overdue Electrical",
                Category = TaskCategory.Electrical,
                Priority = Priority.High,
                Status = Status.Planned,
                PlannedDate = DateTime.Now.AddDays(-2),
                CreatedDate = DateTime.Now.AddDays(-5)
            },
            new()
            {
                Title = "Future Plumbing",
                Category = TaskCategory.Plumbing,
                Priority = Priority.Medium,
                Status = Status.Planned,
                PlannedDate = DateTime.Now.AddDays(5),
                CreatedDate = DateTime.Now.AddDays(-1)
            },
            new()
            {
                Title = "Completed Heating",
                Category = TaskCategory.Heating,
                Priority = Priority.Low,
                Status = Status.Completed,
                PlannedDate = DateTime.Now.AddDays(-10),
                CreatedDate = DateTime.Now.AddDays(-15)
            }
        };

        _context.MaintenanceTasks.AddRange(tasks);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTasks()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetOverdueTasksAsync_ReturnsOnlyOverdueAndNotCompleted()
    {
        // Act
        var result = await _repository.GetOverdueTasksAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Overdue Electrical", result.First().Title);
        Assert.DoesNotContain(result, t => t.Status == Status.Completed);
    }

    [Fact]
    public async Task GetTasksByPriorityAsync_ReturnsFilteredTasks()
    {
        // Act
        var result = await _repository.GetTasksByPriorityAsync(Priority.High);

        // Assert
        Assert.Single(result);
        Assert.Equal("Overdue Electrical", result.First().Title);
        Assert.Equal(Priority.High, result.First().Priority);
    }

    [Fact]
    public async Task GetTasksByCategoryAsync_ReturnsFilteredTasks()
    {
        // Act
        var result = await _repository.GetTasksByCategoryAsync(TaskCategory.Plumbing);

        // Assert
        Assert.Single(result);
        Assert.Equal("Future Plumbing", result.First().Title);
        Assert.Equal(TaskCategory.Plumbing, result.First().Category);
    }

    [Fact]
    public async Task AddAsync_AddsTaskToDatabase()
    {
        // Arrange
        var newTask = new MaintenanceTask
        {
            Title = "New Test Task",
            Category = TaskCategory.Appliances,
            Priority = Priority.Medium,
            Status = Status.Planned,
            PlannedDate = DateTime.Now.AddDays(7)
        };

        // Act
        var result = await _repository.AddAsync(newTask);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Test Task", result.Title);
        Assert.Equal(4, _context.MaintenanceTasks.Count());
    }

    [Fact]
    public async Task DeleteAsync_RemovesTaskFromDatabase()
    {
        // Arrange
        var taskToDelete = _context.MaintenanceTasks.First();

        // Act
        await _repository.DeleteAsync(taskToDelete.Id);

        // Assert
        Assert.Equal(2, _context.MaintenanceTasks.Count());
        Assert.DoesNotContain(_context.MaintenanceTasks, t => t.Id == taskToDelete.Id);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}