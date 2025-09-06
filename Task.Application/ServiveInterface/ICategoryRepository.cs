using System;

using Task.Domain.Entities;
namespace Task.Application.ServiveInterface;

public interface ICategoryRepository
{
    void Add(Category category);
    void Update(Category category);
    void Delete(Guid id);
    Category? GetById(Guid id);
    IEnumerable<Category>? GetAll();


}
