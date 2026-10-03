using AboutGame;

namespace APISearch;

public class ManagingCache
{
    private readonly Dictionary<string, CachedGame> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(30);

    public bool TryGet(string key, out Game? game)
    {
        if (_cache.TryGetValue(key, out CachedGame cached))
        {
            if (DateTime.UtcNow - cached.CachedAtUtc < _expiration)
            {
                game = cached.Game;
                return true;
            }
            _cache.Remove(key);
        }
        game = null;
        return false;
    }

    public void Add(string key, Game game) =>
        _cache[key] = new CachedGame { Game = game, CachedAtUtc = DateTime.UtcNow };
}
