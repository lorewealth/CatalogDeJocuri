using System.Globalization;

namespace WPF.Validators;

/// <summary>Value-only validation shared by the Add Game and Edit Game screens.</summary>
public static class GameInputValidator
{
    public static bool TryParseGameDetails(
        string? name,
        string? priceText,
        DateTime? releaseDate,
        string? genresText,
        string? publishersText,
        string? developersText,
        out GameInput input,
        out string? error)
    {
        input = default;
        if (string.IsNullOrWhiteSpace(name) || releaseDate is null ||
            !decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal price) || price < 0)
        {
            error = "Enter a game name, a non-negative price, and a release date.";
            return false;
        }

        string[] genres = SplitNames(genresText);
        string[] publishers = SplitNames(publishersText);
        string[] developers = SplitNames(developersText);
        if (genres.Length == 0 || publishers.Length == 0 || developers.Length == 0)
        {
            error = "Enter at least one genre, publisher, and developer (comma-separated).";
            return false;
        }

        input = new GameInput(name.Trim(), price, releaseDate.Value, genres, publishers, developers);
        error = null;
        return true;
    }

    public static bool TryValidatePlatforms(bool hasStore, bool hasOperatingSystem, out string? error)
    {
        error = hasStore && hasOperatingSystem ? null : "Select at least one store and one supported operating system."; 
        return error is null;
    }

    public static bool TryParseOptionalScore(string? scoreText, out double? score, out string? error)
    {
        score = null;
        error = null;
        if (string.IsNullOrWhiteSpace(scoreText))
            return true;

        if (!double.TryParse(scoreText, NumberStyles.Number, CultureInfo.CurrentCulture, out double value) || value is < 0 or > 10)
        {
            error = "Score must be a number between 0 and 10.";
            return false;
        }
        score = value;
        return true;
    }

    private static string[] SplitNames(string? value) => value?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Distinct(StringComparer.OrdinalIgnoreCase).ToArray() ?? [];

    public readonly record struct GameInput(string Name, decimal Price, DateTime ReleaseDate, string[] Genres, string[] Publishers, string[] Developers);
}
