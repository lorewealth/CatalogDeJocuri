using AboutGame;
using Stocking.Layouts;

namespace Stocking.Tools
{
    internal class FieldManipulator
    {
        internal static string[]? GetValue(string field, string[]? values = null, Game? game = null)
        {
            if (values == null && game is not null && TXTLayout.SerializeField.TryGetValue(field, out Func<Game, string>? elem))
                return [elem(game)];
            else if (values != null && TXTLayout.DeserializeField.TryGetValue(field, out Func<string[], string[]>? result))
                return result(values);
            else return null;
        }
    }
}
