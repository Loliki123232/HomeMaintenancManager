using HomeMaintenanceManager.Core.Enums;
using HomeMaintenanceManager.Core.Models;
using Moq;

[Fact]
public async Task GetOverdueTasksAsync_Returns_OverdueTasks()
{
    // Arrange
    var currentDate = DateTime.Now;
    var tasks = new List<MaintenanceTask>
    {
        new()
        {
            Id = 1,
            Title = "Overdue Task",
            PlannedDate = currentDate.AddDays(-5), // ПРОСРОЧЕНА на 5 дней
            Status = Status.Planned,
            CompletedDate = null // Не выполнена
        },
        new()
        {
            Id = 2,
            Title = "Future Task",
            PlannedDate = currentDate.AddDays(5), // В будущем
            Status = Status.Planned,
            CompletedDate = null
        },
        new()
        {
            Id = 3,
            Title = "Completed Overdue Task",
            PlannedDate = currentDate.AddDays(-5), // Просрочена но выполнена
            Status = Status.Completed,
            CompletedDate = currentDate.AddDays(-1)
        }
    };

    _mockRepository.Setup(repo => repo.GetAllAsync())
                  .ReturnsAsync(tasks);

    // Act
    var result = await _taskService.GetOverdueTasksAsync();

    // Assert
    Assert.Single(result);
    Assert.Equal("Overdue Task", result.First().Title);
    Assert.Null(result.First().CompletedDate); // Убедимся что не выполнена
}