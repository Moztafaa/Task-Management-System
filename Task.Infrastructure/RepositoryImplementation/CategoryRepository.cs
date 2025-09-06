using System;
using Task.Domain.RepositoryInterface;
using Task.Domain.Entities;
using Task.Infrastructure.DatabaseContext;

namespace Task.Infrastructure.RepositoryImplementation;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public void Add(Category category)
    {
        context.Categories.Add(category);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        Category? existingCategory = context.Categories.Find(id);
        if (existingCategory != null)
        {
            context.Categories.Remove(existingCategory);
            context.SaveChanges();
        }
        else
        {
            throw new ArgumentException("Category not found");
        }
    }

    public IEnumerable<Category>? GetAll()
    {
        return context.Categories.ToList();
    }

    public Category? GetById(Guid id)
    {
        return context.Categories.Find(id);
    }

    public void Update(Category category)
    {
        context.Categories.Update(category);
        context.SaveChanges();
    }
}
