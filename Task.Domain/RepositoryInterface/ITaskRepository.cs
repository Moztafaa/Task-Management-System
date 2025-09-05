using System;
using Task.Domain.Entities;
using TaskStatus = Task.Domain.Entities.TaskStatus;

namespace Task.Domain.RepositoryInterface;

public interface ITaskRepository
{

    void Add(TaskItem task);
    TaskItem? GetById(Guid id);
    IEnumerable<TaskItem>? GetAll();
    void Update(TaskItem task);
    void Delete(Guid id);

    // Searching Functionality

    IEnumerable<TaskItem>? Search(string searchTerm);
    IEnumerable<TaskItem>? SearchByTitle(string title);
    IEnumerable<TaskItem>? SearchByStatus(TaskStatus status);
    IEnumerable<TaskItem>? SearchByPriority(TaskPriority priority);
    IEnumerable<TaskItem>? SearchByCategory(Guid categoryId);
    IEnumerable<TaskItem>? SearchByDateRange(DateTime startDate, DateTime endDate);


    // Reporting Functionality

    int GetTaskCountByStatus(TaskStatus status);
    int GetTaskCountByStatusForUser(TaskStatus status, Guid userId);
    int GetTaskCountByStatusForDateRange(TaskStatus status, DateTime startDate, DateTime endDate);

    IEnumerable<TaskItem> GetTasksByStatus(TaskStatus status);
    IEnumerable<TaskItem> GetTasksByStatusForUser(TaskStatus status, Guid userId);

    IEnumerable<TaskItem> GetOverdueTasks();
    IEnumerable<TaskItem> GetOverdueTasksForUser(Guid userId);

    IEnumerable<TaskItem> GetUpcomingTasks(int days = 7);
    IEnumerable<TaskItem> GetUpcomingTasksForUser(Guid userId, int days = 7);

    IEnumerable<TaskItem> GetCompletedTasksWithDuration();
    IEnumerable<TaskItem> GetCompletedTasksWithDurationForUser(Guid userId);
}
