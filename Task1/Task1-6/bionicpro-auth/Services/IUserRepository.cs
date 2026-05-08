using BionicProAuth.Models;

namespace BionicProAuth.Services;

public interface IUserRepository
{
    Task<User?> FindByYandexIdAsync(string yandexId);
    Task<User?> FindByIdAsync(Guid id);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
}