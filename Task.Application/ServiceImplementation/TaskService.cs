using System;
using System.Linq;
using Task.Application.ServiceInterface;
using Task.Domain.Entities;
using Task.Domain.RepositoryInterface;

namespace Task.Application.ServiceImplementation;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public void AddTask(TaskItem task)
    {
        ValidateTask(task);
        taskRepository.Add(task);
    }

    public void DeleteTask(Guid id)
    {
        TaskItem? existingTask = taskRepository.GetById(id);
        if (existingTask != null)
        {
            taskRepository.Delete(id);
        }
        else
        {
            throw new ArgumentException("Task not found");
        }

    }

    public IEnumerable<TaskItem>? GetAllTasks()
    {
        return taskRepository.GetAll();
    }

    public TaskItem? GetTaskById(Guid id)
    {
        return taskRepository.GetById(id);
    }

    public void UpdateTask(TaskItem task)
    {
        ValidateTask(task);
        TaskItem? existingTask = taskRepository.GetById(task.Id);
        if (existingTask != null)
        {
            taskRepository.Update(task);
        }
        else
        {
            throw new ArgumentException("Task not found");
        }
    }

    // User-specific async methods
    public async System.Threading.Tasks.Task<IEnumerable<TaskItem>> GetUserTasksAsync(Guid userId)
    {
        return await System.Threading.Tasks.Task.FromResult(
            taskRepository.GetAll()?.Where(t => t.UserId == userId) ?? new List<TaskItem>()
        );
    }

    public async System.Threading.Tasks.Task AddTaskAsync(TaskItem task)
    {
        ValidateTask(task);
        await System.Threading.Tasks.Task.Run(() => AddTask(task));
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(TaskItem task)
    {
        ValidateTask(task);
        await System.Threading.Tasks.Task.Run(() => UpdateTask(task));
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(Guid id)
    {
        await System.Threading.Tasks.Task.Run(() => DeleteTask(id));
    }


    private static void ValidateTask(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            throw new ArgumentException("Task title cannot be empty");
        }

        if (task.DueDate.HasValue && task.DueDate < DateTime.Now)
        {
            throw new ArgumentException("Due date cannot be in the past");
        }

        if (task.Title.Length > 200)
        {
            throw new ArgumentException("Task title cannot exceed 200 characters");
        }

        if (task.Description?.Length > 500)
        {
            throw new ArgumentException("Task description cannot exceed 500 characters");
        }
    }

    // Searching Functionality

    public IEnumerable<TaskItem>? SearchTasks(string searchTerm)
    {
        ValidateSearchTerm(searchTerm);
        return taskRepository.Search(searchTerm);
    }

    public IEnumerable<TaskItem>? SearchTasksByTitle(string title)
    {
        ValidateSearchTerm(title);
        return taskRepository.SearchByTitle(title);
    }

    public IEnumerable<TaskItem>? SearchTasksByStatus(Domain.Entities.TaskStatus status)
    {
        return taskRepository.SearchByStatus(status);
    }

    public IEnumerable<TaskItem>? SearchTasksByPriority(TaskPriority priority)
    {
        return taskRepository.SearchByPriority(priority);
    }

    public IEnumerable<TaskItem>? SearchTasksByCategory(Guid categoryId)
    {
        return taskRepository.SearchByCategory(categoryId);
    }

    public IEnumerable<TaskItem>? SearchTasksByDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be after end date");

        return taskRepository.SearchByDateRange(startDate, endDate);
    }

    public static void ValidateSearchTerm(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be empty");

        if (searchTerm.Length < 2)
            throw new ArgumentException("Search term must be at least 2 characters");
    }
}
