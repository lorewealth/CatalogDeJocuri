using AboutGame;
using Stocking.Layouts;
using Stocking.Tools;
using System.Collections.Immutable;
using System.Text;

namespace Stocking.Serialization
{
    public class TXTSerializer : ISerializer<List<Game>>
    {
        public string FileExtension => "txt";

        //methods
        public byte[] Serialize(List<Game> games)
        {
            StringBuilder sb = new();
            ImmutableArray<Func<Game, string>> Actions = TXTLayout.SerializeField.Values;

            for (int i = 0; i < games.Count; i++)
            {
                Game game = games[i];
                int fieldCount = 0;

                foreach (Func<Game, string> action in Actions)
                {
                    sb.Append(action(game));
                    if (++fieldCount < Actions.Length) sb.Append(TXTLayout.FIELD_SEPARATOR);
                }

                if (i < games.Count - 1) sb.Append(TXTLayout.GAME_SEPARATOR);
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
        public List<Game> Deserialize(byte[] data)
        {
            string decoded = Encoding.UTF8.GetString(data);

            string[] deserialized = decoded.Split(TXTLayout.GAME_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);

            List<Game> games = [.. deserialized
                .Select(element => Builder.BuildGame(element.Split(TXTLayout.FIELD_SEPARATOR)))
                .OfType<Game>()];

            return games;
        }
    }
}
