using AboutGame;

namespace APISearch;

internal struct CachedGame
{
    public Game Game { get; set; }
    public DateTime CachedAtUtc { get; set; }
}
