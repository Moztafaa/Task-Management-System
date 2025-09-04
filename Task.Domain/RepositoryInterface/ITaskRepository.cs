using System;
using Task.Domain.Entities;

namespace Task.Domain.RepositoryInterface;

public interface ITaskRepository
{

    void Add(TaskItem task);
    TaskItem? GetById(Guid id);
    IEnumerable<TaskItem>? GetAll();
    void Update(TaskItem task);
    void Delete(Guid id);

}
