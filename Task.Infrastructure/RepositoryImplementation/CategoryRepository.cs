using System;
using Microsoft.EntityFrameworkCore;
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
        // First, detach any existing tracked entity with the same ID
        var trackedEntities = context.ChangeTracker.Entries<Category>()
            .Where(e => e.Entity.Id == category.Id)
            .ToList();

        foreach (var trackedEntity in trackedEntities)
        {
            trackedEntity.State = EntityState.Detached;
        }

        // Find the existing entity in the database and update it
        var existingCategory = context.Categories.Find(category.Id);
        if (existingCategory != null)
        {
            // Update properties
            context.Entry(existingCategory).CurrentValues.SetValues(category);
        }
        else
        {
            // If not found, add as new (shouldn't happen in normal update scenario)
            context.Categories.Update(category);
        }

        context.SaveChanges();
    }
}
