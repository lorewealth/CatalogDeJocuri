using AboutGame;
using System.Text.Json;

namespace Stocking.Serialization;

public class JSONSerializer : ISerializer<List<Game>>
{
    public string FileExtension => "json";

    public byte[] Serialize(List<Game> games) =>
        JsonSerializer.SerializeToUtf8Bytes(games, SerializerOptions);

    public List<Game> Deserialize(byte[] data) =>
        JsonSerializer.Deserialize<List<Game>>(data, SerializerOptions) ?? [];

    internal static JsonSerializerOptions SerializerOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
}
