using System;
using Task.Domain.Entities;

namespace Task.Application.ServiveInterface;

public interface ITaskService
{

    void AddTask(TaskItem task);
    TaskItem? GetTaskById(Guid id);
    IEnumerable<TaskItem>? GetAllTasks();
    void UpdateTask(TaskItem task);
    void DeleteTask(Guid id);


    // Searching Functionality

    IEnumerable<TaskItem>? SearchTasks(string searchTerm);
    IEnumerable<TaskItem>? SearchTasksByTitle(string title);
    IEnumerable<TaskItem>? SearchTasksByStatus(Domain.Entities.TaskStatus status);
    IEnumerable<TaskItem>? SearchTasksByPriority(TaskPriority priority);
    IEnumerable<TaskItem>? SearchTasksByCategory(Guid categoryId);
    IEnumerable<TaskItem>? SearchTasksByDateRange(DateTime startDate, DateTime endDate);


}
