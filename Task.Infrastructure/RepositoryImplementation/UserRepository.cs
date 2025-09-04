using System;
using Task.Domain.Entities;
using Task.Domain.RepositoryInterface;
using Task.Infrastructure.DatabaseContext;

namespace Task.Infrastructure.RepositoryImplementation;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public void Add(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        User? user = GetById(id);
        if (user != null)
        {
            context.Users.Remove(user);
            context.SaveChanges();
        }
        else
        {
            throw new ArgumentException("User not found");
        }
    }

    public bool ExistsByEmail(string email)
    {
        return context.Users.Any(u => u.Email == email);
    }

    public bool ExistsByUsername(string username)
    {
        return context.Users.Any(u => u.Username == username);
    }

    public IEnumerable<User> GetAll()
    {
        return context.Users.ToList();
    }

    public User? GetByEmail(string email)
    {
        return context.Users.FirstOrDefault(u => u.Email == email);
    }

    public User? GetById(Guid id)
    {
        return context.Users.Find(id);
    }

    public User? GetByUsername(string username)
    {
        return context.Users.FirstOrDefault(u => u.Username == username);
    }

    public void Update(User user)
    {
        context.Users.Update(user);
        context.SaveChanges();
    }
}
