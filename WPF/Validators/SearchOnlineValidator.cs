using AboutGame;

namespace WPF.Validators;

/// <summary>Value-only validation for online game search and import.</summary>
public static class OnlineSearchValidator
{
    public static bool TryValidateSearchText(string? searchText, out string? error)
    {
        error = string.IsNullOrWhiteSpace(searchText) ? "Enter a game title to search." : null;
        return error is null;
    }

    public static bool IsAlreadyInCatalog(Game game, IEnumerable<Game> catalog) =>
        !string.IsNullOrWhiteSpace(game.ExternalId) &&
        catalog.Any(item => string.Equals(item.ExternalId, game.ExternalId, StringComparison.OrdinalIgnoreCase));
}
