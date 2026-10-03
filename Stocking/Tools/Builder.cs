using AboutGame;
using Stocking.Enums;
using Stocking.Layouts;
using Stocking.Validators;

namespace Stocking.Tools
{
    internal class Builder
    {
        internal static Game? BuildGame(string[] values)
        {
            if (!SerializerValidator.Check(values)) return null;

            Dictionary<int, string[]> result = [];
            int idx = 0;

            foreach (string field in TXTLayout.DeserializeField.Keys)
            {
                idx++;
                string[] data = TXTLayout.DeserializeField[field](values);
                result[idx] = data;
            }

            List<Genre> genres         = ListConstructor(result, FieldIDXs.Genres, name => new Genre(0, name));
            List<Publisher> publishers = ListConstructor(result, FieldIDXs.Publishers, name => new Publisher(0, name));
            List<Developer> developers = ListConstructor(result, FieldIDXs.Developers, name => new Developer(0, name));


            Game game = new() 
            {
                InternalId  = Convert.ToInt32(result[(int)FieldIDXs.InternalId][0]),
                ExternalId  = result[(int)FieldIDXs.ExternalId][0],
                Name        = result[(int)FieldIDXs.Name][0],
                Price       = Convert.ToDecimal(result[(int)FieldIDXs.Price][0]), 
                Genres      = genres,
                Publishers  = publishers,
                Developers  = developers,
                ReleaseDate = Convert.ToDateTime(result[(int)FieldIDXs.ReleaseDate][0]),
                IsAvailable = Convert.ToBoolean(result[(int)FieldIDXs.IsAvailable][0]),
                ImgUrl      = result[(int)FieldIDXs.ImgUrl][0]
            };

            return game;
        }
            
        public static List<T> ListConstructor<T>(Dictionary<int, string[]> result, FieldIDXs idx, Func<string, T> factory) where T: IIdentifiable
        {
            HashSet<string> uniquity = [.. result[(int)idx]];
            return [.. uniquity.Select(factory)];
        }
    }
}
