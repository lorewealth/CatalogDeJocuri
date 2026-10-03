using AboutGame.Enums;
using Stocking.Layouts;
using Stocking.Enums;
using System.Globalization;
using AboutGame.Constants;

namespace Stocking.Validators
{
    public static class SerializerValidator
    {
        public static bool Check(string[] parts)
        {
            if (parts.Length != TXTLayout.SerializeField.Count) return false;
            
            if (!int.TryParse(parts[(int)FieldIDXs.InternalId], out int internalId)
                    || internalId < 0) return false;
            
            if (string.IsNullOrWhiteSpace(parts[(int)FieldIDXs.Name])) return false;
            
            if (!decimal.TryParse(parts[(int)FieldIDXs.Price], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price)
                    || price < 0) return false;
            
            if (!DateTime.TryParseExact(parts[(int)FieldIDXs.ReleaseDate], "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime releaseDate)
                || (releaseDate.Year < GameConstants.MIN_YEAR)
                || (releaseDate.Year > GameConstants.MAX_YEAR)) return false;
            
            if (string.IsNullOrWhiteSpace(parts[(int)FieldIDXs.AgeRatings])) return false;
            string[] ratingsParts = parts[(int)FieldIDXs.AgeRatings].Split(TXTLayout.LIST_SEPARATOR);
            if (ratingsParts.Any(string.IsNullOrWhiteSpace)) return false;
            foreach (string elem in ratingsParts)
            {
                string[] ratingSubParts = elem.Split(TXTLayout.RATING_SEPARATOR);
                if (ratingSubParts.Length != 2) return false;
                if (!Enum.IsDefined(typeof(AgeRatingsCategory), ratingSubParts[0].Trim())
                    || !Enum.IsDefined(typeof(AgeRatingsValue), ratingSubParts[1].Trim()))
                    return false;
            }
            
            if (string.IsNullOrWhiteSpace(parts[(int)FieldIDXs.SupportedOS])) return false;
            foreach (string elem in parts[(int)FieldIDXs.SupportedOS].Split(TXTLayout.LIST_SEPARATOR))
            {
                if (!Enum.TryParse<SupportedOS>(elem.Trim(), true, out SupportedOS parsed)
                    || (parsed & ~SupportedOS.All) != 0) return false;
            
            }
            
            if (string.IsNullOrWhiteSpace(parts[(int)FieldIDXs.Scores])) return false;
            foreach (string elem in parts[(int)FieldIDXs.Scores].Split(TXTLayout.LIST_SEPARATOR))
            {
                string[] subparts = elem.Split(TXTLayout.RATING_SEPARATOR);
                if (subparts.Length != 2) return false;
            
                if (!Enum.TryParse(subparts[0], true, out RatingSource Source)) return false;
                if (!double.TryParse(subparts[1].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out double value)) return false;
            
                if (value > (double)Source || value < 1) return false;
            }
            
            if (string.IsNullOrWhiteSpace(parts[(int)FieldIDXs.Stores])) return false;
            foreach (string elem in parts[(int)FieldIDXs.Stores].Split(TXTLayout.LIST_SEPARATOR))
            {
                if (!Enum.TryParse<AvailableStores>(elem.Trim(), true, out AvailableStores st)
                    || (st & ~AvailableStores.All) != 0) return false;
            }
            
            if (InvalidListField(parts, (int)FieldIDXs.Genres)) return false;
            
            if (InvalidListField(parts, (int)FieldIDXs.Publishers)) return false;
            
            if (InvalidListField(parts, (int)FieldIDXs.Developers)) return false;
            
            if (!bool.TryParse(parts[(int)FieldIDXs.IsAvailable], out _)) return false;
            
            return true;
        }
        private static bool InvalidListField(string[] parts, int idx)
        {
            if (string.IsNullOrWhiteSpace(parts[idx])) return true;
            string[] PartElem = parts[idx].Split(TXTLayout.LIST_SEPARATOR);
            return PartElem.Any(string.IsNullOrWhiteSpace);
        }
    }
}
