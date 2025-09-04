using System;
using Task.Application.ServiveInterface;
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
}
