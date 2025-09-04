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

}
