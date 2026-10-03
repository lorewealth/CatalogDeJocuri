using AboutGame.Constants;
using System.Globalization;

namespace WPF.Validators;

/// <summary>Value-only validation for the internal catalog search.</summary>
public static partial class SearchValidator
{
    public static bool TryGetRequiredText(string? value, string field, out string? result, out string? error)
    {
        result = value?.Trim();
        error = string.IsNullOrWhiteSpace(result) ? $"Enter a {field}." : null;
        return error is null;
    }

    public static bool TryGetTerms(string? value, string field, out string[] terms, out string? error)
    {
        terms = value?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
        error = terms.Length == 0 ? $"Enter at least one {field}." : null;
        return error is null;
    }

    public static bool TryGetRange(string? value, string field, decimal minimum, decimal maximum, out DecimalRange? range, out string? error)
    {
        range = null;
        error = null;
        string[] parts = value?.Split('-', StringSplitOptions.TrimEntries) ?? [];
        if (parts.Length != 2 ||
            !decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.CurrentCulture, out decimal min) ||
            !decimal.TryParse(parts[1], NumberStyles.Number, CultureInfo.CurrentCulture, out decimal max) ||
            min < minimum || max > maximum || min > max)
        {
            error = $"Enter a valid {field} range as min-max.";
            return false;
        }

        range = new DecimalRange(min, max);
        return true;
    }

    public static bool TryGetYearRange(string? value, out IntRange? range, out string? error)
    {
        range = null;
        error = null;
        string[] parts = value?.Split('-', StringSplitOptions.TrimEntries) ?? [];
        if (parts.Length != 2 || !int.TryParse(parts[0], out int min) || !int.TryParse(parts[1], out int max) ||
            min < GameConstants.MIN_YEAR || max > GameConstants.MAX_YEAR || min > max)
        {
            error = $"Enter a release-year range as {GameConstants.MIN_YEAR}-{GameConstants.MAX_YEAR}.";
            return false;
        }

        range = new IntRange(min, max);
        return true;
    }

    public readonly record struct DecimalRange(decimal Min, decimal Max);
    public readonly record struct IntRange(int Min, int Max);
}
