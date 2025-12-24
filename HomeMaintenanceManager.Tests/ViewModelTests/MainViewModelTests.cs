using HomeMaintenanceManager.Core.Models;
using Moq;

[Fact]
public async Task DeleteTaskCommand_RemovesTaskFromCollection()
{
    // Arrange
    var task = new MaintenanceTask
    {
        Id = 1,
        Title = "Task to delete"
    };

    var tasks = new List<MaintenanceTask> { task };
    _mockService.Setup(s => s.GetAllTasksAsync())
               .ReturnsAsync(tasks);
    _mockService.Setup(s => s.GetOverdueTasksAsync())
               .ReturnsAsync(new List<MaintenanceTask>());
    _mockService.Setup(s => s.GetUpcomingTasksAsync(It.IsAny<int>()))
               .ReturnsAsync(new List<MaintenanceTask>());

    await _viewModel.LoadTasks();

    // Настройка мока для удаления
    _mockService.Setup(s => s.DeleteTaskAsync(1))
               .Returns(Task.CompletedTask)
               .Callback(() => tasks.Remove(task)); // Удаляем задачу из списка

    // Act
    if (_viewModel.DeleteTaskCommand.CanExecute(task))
    {
        _viewModel.DeleteTaskCommand.Execute(task);
    }

    // Ждем завершения асинхронной операции
    await Task.Delay(100);

    // Assert
    _mockService.Verify(s => s.DeleteTaskAsync(1), Times.Once);
    Assert.DoesNotContain(task, _viewModel.Tasks);
}