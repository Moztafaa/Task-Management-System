using System;
using Microsoft.EntityFrameworkCore;
using Task.Domain.Entities;
using Task.Domain.RepositoryInterface;
using Task.Infrastructure.DatabaseContext;

namespace Task.Infrastructure.RepositoryImplementation;

public class TaskRepository(AppDbContext context) : ITaskRepository
{

    public void Add(TaskItem task)
    {
        context.Tasks.Add(task);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        TaskItem? task = context.Tasks.Find(id);
        if (task != null)
        {
            context.Tasks.Remove(task);
            context.SaveChanges();
        }
    }

    public IEnumerable<TaskItem>? GetAll()
    {
        return context.Tasks.AsNoTracking().ToList();
    }

    public IEnumerable<TaskItem>? GetTasksByUser(Guid userId)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .ToList();
    }

    public TaskItem? GetById(Guid id)
    {
        return context.Tasks.AsNoTracking().FirstOrDefault(t => t.Id == id);
    }
    public void Update(TaskItem task)
    {
        // First, detach any existing tracked entity with the same ID
        var trackedEntities = context.ChangeTracker.Entries<TaskItem>()
            .Where(e => e.Entity.Id == task.Id)
            .ToList();

        foreach (var trackedEntity in trackedEntities)
        {
            trackedEntity.State = EntityState.Detached;
        }

        // Find the existing entity in the database and update it
        var existingTask = context.Tasks.Find(task.Id);
        if (existingTask != null)
        {
            // Update properties
            context.Entry(existingTask).CurrentValues.SetValues(task);
        }
        else
        {
            // If not found, add as new (shouldn't happen in normal update scenario)
            context.Tasks.Update(task);
        }

        context.SaveChanges();
    }

    // Searching Functionality

    public IEnumerable<TaskItem>? Search(string searchTerm)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.Title!.Contains(searchTerm) || (t.Description != null && t.Description.Contains(searchTerm)))
            .ToList();
    }

    public IEnumerable<TaskItem>? SearchByCategory(Guid categoryId)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.CategoryId == categoryId)
            .ToList();
    }

    public IEnumerable<TaskItem>? SearchByDateRange(DateTime startDate, DateTime endDate)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.DueDate >= startDate && t.DueDate <= endDate)
            .ToList();
    }

    public IEnumerable<TaskItem>? SearchByPriority(TaskPriority priority)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.Priority == priority)
            .ToList();
    }

    public IEnumerable<TaskItem>? SearchByStatus(Domain.Entities.TaskStatus status)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.Status == status)
            .ToList();
    }

    public IEnumerable<TaskItem>? SearchByTitle(string title)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.Title!.Contains(title))
            .ToList();
    }


    // Reporting Functionality
    public int GetTaskCountByStatus(Domain.Entities.TaskStatus status)
    {
        return context.Tasks.Count(t => t.Status == status);
    }

    public int GetTaskCountByStatusForUser(Domain.Entities.TaskStatus status, Guid userId)
    {
        return context.Tasks.Count(t => t.Status == status && t.UserId == userId);
    }

    public int GetTaskCountByStatusForDateRange(Domain.Entities.TaskStatus status, DateTime startDate, DateTime endDate)
    {
        return context.Tasks.Count(t => t.Status == status &&
                                     t.CreatedAt >= startDate &&
                                     t.CreatedAt <= endDate);

    }

    public IEnumerable<TaskItem> GetTasksByStatus(Domain.Entities.TaskStatus status)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.Status == status)
            .Include(t => t.AssignedUser)
            .Include(t => t.Category).ToList();
    }

    public IEnumerable<TaskItem> GetTasksByStatusForUser(Domain.Entities.TaskStatus status, Guid userId)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.Status == status && t.UserId == userId).Include(t => t.Category).ToList();
    }

    public IEnumerable<TaskItem> GetOverdueTasks()
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.DueDate.HasValue && t.DueDate < DateTime.Now && t.Status != Domain.Entities.TaskStatus.Completed)
            .Include(t => t.AssignedUser)
            .Include(t => t.Category).ToList();
    }

    public IEnumerable<TaskItem> GetOverdueTasksForUser(Guid userId)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.DueDate.HasValue &&
                        t.DueDate < DateTime.Now &&
                        t.Status != Domain.Entities.TaskStatus.Completed &&
                        t.UserId == userId)
            .Include(t => t.Category).ToList();
    }

    public IEnumerable<TaskItem> GetUpcomingTasks(int days = 7)
    {
        DateTime targetDate = DateTime.Now.AddDays(days);
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.DueDate.HasValue &&
                        t.DueDate >= DateTime.Now &&
                        t.DueDate <= targetDate &&
                        t.Status != Domain.Entities.TaskStatus.Completed)
            .Include(t => t.AssignedUser)
            .Include(t => t.Category)
            .OrderBy(t => t.DueDate)
            .ToList();
    }

    public IEnumerable<TaskItem> GetUpcomingTasksForUser(Guid userId, int days = 7)
    {
        DateTime targetDate = DateTime.Now.AddDays(days);
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.DueDate.HasValue &&
                        t.DueDate >= DateTime.Now &&
                        t.DueDate <= targetDate &&
                        t.Status != Domain.Entities.TaskStatus.Completed &&
                        t.UserId == userId)
            .Include(t => t.Category)
            .OrderBy(t => t.DueDate)
            .ToList();
    }

    public IEnumerable<TaskItem> GetCompletedTasksWithDuration()
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.Status == Domain.Entities.TaskStatus.Completed)
            .Include(t => t.AssignedUser)
            .Include(t => t.Category)
            .ToList();
    }

    public IEnumerable<TaskItem> GetCompletedTasksWithDurationForUser(Guid userId)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(t => t.Status == Domain.Entities.TaskStatus.Completed && t.UserId == userId)
            .Include(t => t.Category)
            .ToList();
    }


}
