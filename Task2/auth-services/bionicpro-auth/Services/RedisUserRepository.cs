using BionicProAuth.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace BionicProAuth.Services;

public class RedisUserRepository : IUserRepository
{
    private readonly IDatabase _db;

    public RedisUserRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<User?> FindByYandexIdAsync(string yandexId)
    {
        var userId = await _db.StringGetAsync(YandexKey(yandexId));

        if (userId.IsNullOrEmpty)
            return null;

        return await FindByIdAsync(Guid.Parse(userId!));
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        var json = await _db.StringGetAsync(UserKey(id));

        if (json.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<User>(json!);
    }

    public async Task CreateAsync(User user)
    {
        var json = JsonSerializer.Serialize(user);

        var batch = _db.CreateBatch();

        batch.StringSetAsync(UserKey(user.Id), json);
        batch.StringSetAsync(YandexKey(user.YandexId), user.Id.ToString());

        batch.Execute();
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(User user)
    {
        var json = JsonSerializer.Serialize(user);

        await _db.StringSetAsync(UserKey(user.Id), json);
    }

    private static string UserKey(Guid id) => $"user:{id}";

    private static string YandexKey(string yandexId) => $"user:yandex:{yandexId}";
}