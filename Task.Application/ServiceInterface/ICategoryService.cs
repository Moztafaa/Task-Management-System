using System;
using Task.Domain.Entities;
namespace Task.Application.ServiceInterface;

public interface ICategoryService
{
    void AddCategory(Category category);
    Category? GetCategoryById(Guid id);
    IEnumerable<Category>? GetAllCategories();
    void UpdateCategory(Category category);
    void DeleteCategory(Guid id);
}
