using AboutGame;
using System.Text;
using System.Text.Json;

namespace Stocking.Serialization;

public class CSVSerializer : ISerializer<List<Game>>
{
    public string FileExtension => "csv";

    public byte[] Serialize(List<Game> games)
    {
        StringBuilder csv = new("GameJson\n");
        foreach (Game game in games)
        {
            string json = JsonSerializer.Serialize(game, JSONSerializer.SerializerOptions);
            csv.Append('"').Append(json.Replace("\"", "\"\"")).AppendLine("\"");
        }
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public List<Game> Deserialize(byte[] data)
    {
        string csv = Encoding.UTF8.GetString(data);
        string[] rows = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (rows.Length == 0 || !string.Equals(rows[0], "GameJson", StringComparison.Ordinal))
            throw new InvalidDataException("This is not a Game Catalog CSV file.");

        List<Game> games = [];
        foreach (string row in rows.Skip(1))
        {
            if (row.Length < 2 || row[0] != '"' || row[^1] != '"')
                throw new InvalidDataException("A game row in the CSV file is invalid.");
            string json = row[1..^1].Replace("\"\"", "\"");
            Game? game = JsonSerializer.Deserialize<Game>(json, JSONSerializer.SerializerOptions);
            if (game is not null) games.Add(game);
        }
        return games;
    }
}
