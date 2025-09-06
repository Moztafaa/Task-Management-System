using System;
using Task.Domain.Entities;

namespace Task.Application.ServiceInterface;

public interface ITaskService
{

    void AddTask(TaskItem task);
    TaskItem? GetTaskById(Guid id);
    IEnumerable<TaskItem>? GetAllTasks();
    void UpdateTask(TaskItem task);
    void DeleteTask(Guid id);

    // User-specific task methods
    System.Threading.Tasks.Task<IEnumerable<TaskItem>> GetUserTasksAsync(Guid userId);
    System.Threading.Tasks.Task AddTaskAsync(TaskItem task);
    System.Threading.Tasks.Task UpdateTaskAsync(TaskItem task);
    System.Threading.Tasks.Task DeleteTaskAsync(Guid id);

    // Searching Functionality

    IEnumerable<TaskItem>? SearchTasks(string searchTerm);
    IEnumerable<TaskItem>? SearchTasksByTitle(string title);
    IEnumerable<TaskItem>? SearchTasksByStatus(Domain.Entities.TaskStatus status);
    IEnumerable<TaskItem>? SearchTasksByPriority(TaskPriority priority);
    IEnumerable<TaskItem>? SearchTasksByCategory(Guid categoryId);
    IEnumerable<TaskItem>? SearchTasksByDateRange(DateTime startDate, DateTime endDate);


}
