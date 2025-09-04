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

    public TaskItem? GetById(Guid id)
    {
        return context.Tasks.AsNoTracking().FirstOrDefault(t => t.Id == id);
    }

    public void Update(TaskItem task)
    {
        context.Tasks.Update(task);
        context.SaveChanges();
    }
}
