using AboutGame;
using System.IO.Compression;
using System.Text.Json;

namespace Stocking.Serialization;

public class BINSerializer : ISerializer<List<Game>>
{
    public string FileExtension => "bin";

    public byte[] Serialize(List<Game> games)
    {
        byte[] json = JsonSerializer.SerializeToUtf8Bytes(games, JSONSerializer.SerializerOptions);
        using MemoryStream output = new();
        using (GZipStream gzip = new(output, CompressionLevel.SmallestSize, leaveOpen: true))
            gzip.Write(json);
        return output.ToArray();
    }

    public List<Game> Deserialize(byte[] data)
    {
        using MemoryStream input = new(data);
        using GZipStream gzip = new(input, CompressionMode.Decompress);
        return JsonSerializer.Deserialize<List<Game>>(gzip, JSONSerializer.SerializerOptions) ?? [];
    }
}
