using System;
using Task.Application.ServiveInterface;
using Task.Domain.Entities;

namespace Task.Application.ServiceImplementation;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public void AddCategory(Category category)
    {
        categoryRepository.Add(category);
    }

    public void DeleteCategory(Guid id)
    {
        categoryRepository.Delete(id);
    }

    public IEnumerable<Category>? GetAllCategories()
    {
        return categoryRepository.GetAll();
    }

    public Category? GetCategoryById(Guid id)
    {
        return categoryRepository.GetById(id);
    }

    public void UpdateCategory(Category category)
    {
        categoryRepository.Update(category);
    }
}
