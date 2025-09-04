using System;
using Task.Domain.Entities;

namespace Task.Domain.RepositoryInterface;

public interface IUserRepository
{

    void Add(User user);
    User? GetById(Guid id);
    User? GetByUsername(string username);
    User? GetByEmail(string email);
    IEnumerable<User> GetAll();
    void Update(User user);
    void Delete(Guid id);
    bool ExistsByUsername(string username);
    bool ExistsByEmail(string email);

}
