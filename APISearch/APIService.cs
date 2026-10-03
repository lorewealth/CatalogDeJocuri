using AboutGame;
using AboutGame.Enums;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace APISearch;

public static class APIService
{
    private static readonly HttpClient Client = new();
    private const string BaseUrl = "https://game-backend.lorewealth-dev.workers.dev/api";

    public static Task<GameSearchResult[]?> SearchAsync(string name) =>
        Client.GetFromJsonAsync<GameSearchResult[]>($"{BaseUrl}/search?name={Uri.EscapeDataString(name)}");

    public static async Task<Game?> RetrieveAsync(int id, string country)
    {
        string url = $"{BaseUrl}/game?id={id}&country={Uri.EscapeDataString(country)}";
        using JsonDocument document = JsonDocument.Parse(await Client.GetStringAsync(url));
        JsonElement root = document.RootElement;
        if (root.ValueKind == JsonValueKind.Array)
        {
            if (root.GetArrayLength() == 0)
                return null;
            root = root[0];
        }
        if (root.ValueKind != JsonValueKind.Object)
            return null;

        string? name = GetString(root, "name");
        if (string.IsNullOrWhiteSpace(name))
            return null;

        int igdbId = GetInt(root, "igdbId");
        // ITAD IDs can represent a shared catalog/edition entry.  IGDB IDs identify
        // individual games, so they are the correct key for the local unique column.
        string externalId = igdbId > 0
            ? $"igdb:{igdbId.ToString(CultureInfo.InvariantCulture)}"
            : $"itad:{GetString(root, "itadId") ?? name}";
        decimal price = GetDecimal(root, "price");

        return new Game
        {
            ExternalId = externalId,
            Name = name,
            Price = price,
            Genres = GetNames(root, "genres").Select(value => new Genre(0, value)).ToList(),
            Developers = GetNames(root, "developers").Select(value => new Developer(0, value)).ToList(),
            Publishers = GetNames(root, "publishers").Select(value => new Publisher(0, value)).ToList(),
            AgeRatings = GetAgeRatings(root),
            Scores = GetScores(root),
            Stores = GetFlags<AvailableStores>(root, "stores"),
            SupportedOS = SupportedOS.Windows,
            ReleaseDate = GetDate(root, "releaseDate"),
            IsAvailable = true,
            ImgUrl = GetString(root, "imgUrl")
        };
    }

    private static IEnumerable<string> GetNames(JsonElement root, string property) =>
        TryGetProperty(root, property, out JsonElement values) && values.ValueKind == JsonValueKind.Array
            ? values.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.String)
                .Select(item => item.GetString()).OfType<string>()
            : [];

    private static List<AgeRating> GetAgeRatings(JsonElement root)
    {
        if (!TryGetProperty(root, "ageRatings", out JsonElement values) || values.ValueKind != JsonValueKind.Array)
            return [];

        List<AgeRating> ratings = [];
        foreach (JsonElement value in values.EnumerateArray())
        {
            string source = GetString(value, "source") ?? "";
            string age = (GetString(value, "value") ?? "").Replace(' ', '_');
            if (TryParseEnum(source, out AgeRatingsCategory category) &&
                TryParseEnum(age, out AgeRatingsValue ageValue))
                ratings.Add(new AgeRating(category, ageValue));
        }
        return ratings;
    }

    private static List<Rating> GetScores(JsonElement root)
    {
        if (!TryGetProperty(root, "scores", out JsonElement values) || values.ValueKind != JsonValueKind.Array)
            return [];

        List<Rating> scores = [];
        foreach (JsonElement value in values.EnumerateArray())
        {
            string source = GetString(value, "source") ?? "";
            if (TryParseEnum(source, out RatingSource ratingSource))
                scores.Add(new Rating(ratingSource, GetDouble(value, "score"), GetString(value, "url") ?? "none", GetInt(value, "count")));
        }
        return scores;
    }

    private static TEnum GetFlags<TEnum>(JsonElement root, string property) where TEnum : struct, Enum
    {
        long result = 0;
        foreach (string value in GetNames(root, property))
            if (TryParseEnum(value, out TEnum parsed))
                result |= Convert.ToInt64(parsed);
        return (TEnum)Enum.ToObject(typeof(TEnum), result);
    }

    private static bool TryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct, Enum
    {
        string normalized = NormalizeEnumName(value);
        foreach (TEnum candidate in Enum.GetValues<TEnum>())
        {
            if (NormalizeEnumName(candidate.ToString()) == normalized)
            {
                result = candidate;
                return true;
            }
        }
        result = default;
        return false;
    }

    private static string NormalizeEnumName(string value) => new string(value.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

    private static DateTime GetDate(JsonElement root, string property) =>
        DateTime.TryParse(GetString(root, property), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date)
            ? date : DateTime.Today;
    private static decimal GetDecimal(JsonElement root, string property) =>
        TryGetProperty(root, property, out JsonElement value) && value.TryGetDecimal(out decimal result) ? result : 0m;
    private static double GetDouble(JsonElement root, string property) =>
        TryGetProperty(root, property, out JsonElement value) && value.TryGetDouble(out double result) ? result : 0d;
    private static int GetInt(JsonElement root, string property)
    {
        if (!TryGetProperty(root, property, out JsonElement value))
            return 0;
        if (value.TryGetInt32(out int result))
            return result;
        return value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out result) ? result : 0;
    }
    private static string? GetString(JsonElement root, string property) =>
        TryGetProperty(root, property, out JsonElement value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    private static bool TryGetProperty(JsonElement root, string property, out JsonElement value) =>
        root.TryGetProperty(property, out value) || root.TryGetProperty(char.ToUpperInvariant(property[0]) + property[1..], out value);
}
