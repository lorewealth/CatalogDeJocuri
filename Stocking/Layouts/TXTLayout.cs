using AboutGame;
using Stocking.Enums;
using System.Collections.Frozen;
namespace Stocking.Layouts
{
    public class TXTLayout
    {
        public const char FIELD_SEPARATOR = '|';
        public const char LIST_SEPARATOR = ';';
        public const char RATING_SEPARATOR = '^';
        public const char GAME_SEPARATOR = '\n';
        public static readonly FrozenDictionary<string, Func<Game, string>> SerializeField = new Dictionary<string, Func<Game, string>>
        {
            ["InternalId"]  = g => g.InternalId.ToString(),
            ["ExternalId"]  = g => g.ExternalId ?? string.Empty, 
            ["Name"]        = g => g.Name,
            ["Price"]       = g => g.Price.ToString(),
            ["Genres"]      = g => string.Join(LIST_SEPARATOR, g.Genres.Select(ge => ge.Name)),
            ["Publishers"]  = g => string.Join(LIST_SEPARATOR, g.Publishers.Select(p => p.Name)),
            ["Developers"]  = g => string.Join(LIST_SEPARATOR, g.Developers.Select(d => d.Name)),
            ["AgeRatings"]  = g => string.Join(LIST_SEPARATOR, g.AgeRatings.Select(a => $"{a.Source}{RATING_SEPARATOR}{a.Age}")),
            ["Scores"]      = g => string.Join(LIST_SEPARATOR, g.Scores.Select(sc => $"{sc.Source}{RATING_SEPARATOR}{sc.RawScore}")),
            ["Stores"]      = g => g.Stores.ToString().Replace(',', LIST_SEPARATOR).Trim(),
            ["SupportedOS"] = g => g.SupportedOS.ToString().Replace(',', LIST_SEPARATOR).Trim(), 
            ["ReleaseDate"] = g => g.ReleaseDate.ToString("yyyy-MM-dd"),
            ["IsAvailable"] = g => g.IsAvailable.ToString(),
            ["ImgUrl"]      = g => g.ImgUrl ?? string.Empty,
        }.ToFrozenDictionary();
        public static readonly FrozenDictionary<string, Func<string[], string[]>> DeserializeField = new Dictionary<string, Func<string[], string[]>>
        {
            ["InternalId"]  = f => [f[(int)FieldIDXs.InternalId]],
            ["ExternalId"]  = f => [f[(int)FieldIDXs.ExternalId]],
            ["Name"]        = f => [f[(int)FieldIDXs.Name]],
            ["Price"]       = f => [f[(int)FieldIDXs.Price]],
            ["Genres"]      = f => f[(int)FieldIDXs.Genres].Split(LIST_SEPARATOR, StringSplitOptions.RemoveEmptyEntries),
            ["Publishers"]  = f => f[(int)FieldIDXs.Publishers].Split(LIST_SEPARATOR, StringSplitOptions.RemoveEmptyEntries),
            ["Developers"]  = f => f[(int)FieldIDXs.Developers].Split(LIST_SEPARATOR, StringSplitOptions.RemoveEmptyEntries),
            ["AgeRatings"]  = f => [.. f[(int)FieldIDXs.AgeRatings].Split(LIST_SEPARATOR, StringSplitOptions.RemoveEmptyEntries).SelectMany(age => age.Split(RATING_SEPARATOR))],
            ["Scores"]      = f => [.. f[(int)FieldIDXs.Scores].Split(LIST_SEPARATOR, StringSplitOptions.RemoveEmptyEntries).SelectMany(score => score.Split(RATING_SEPARATOR))],
            ["Stores"]      = f => f[(int)FieldIDXs.Stores].Split(LIST_SEPARATOR, StringSplitOptions.RemoveEmptyEntries),
            ["SupportedOS"] = f => f[(int)FieldIDXs.SupportedOS].Split(LIST_SEPARATOR, StringSplitOptions.RemoveEmptyEntries),
            ["ReleaseDate"] = f => [f[(int)FieldIDXs.ReleaseDate]],
            ["IsAvailable"] = f => [f[(int)FieldIDXs.IsAvailable]],
            ["ImgUrl"]      = f => [f[(int)FieldIDXs.ImgUrl]]
        }.ToFrozenDictionary();
    }
}