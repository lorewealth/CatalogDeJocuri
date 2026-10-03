using Stocking.Serialization;
using System.IO.Compression;
using System.Text.Json;
using AboutGame;
using Stocking.Tools;

namespace Stocking.Packaging
{
    public class GCATImporter
    {
        private readonly Dictionary<string, ISerializer<List<Game>>> _serializers;
        
        public GCATImporter(IEnumerable<ISerializer<List<Game>>> serializers)
        {
            _serializers = serializers.ToDictionary(s => s.FileExtension, s => s);
        }

        public async Task<(GCATManifest Manifest, GCATPackage Package)> ImportAsync(string sourceDir, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                using var archive = ZipFile.OpenRead(sourceDir);
                
                GCATManifest manifest = JsonSerializer.Deserialize<GCATManifest>(ImportTools.ReadEntry(archive, "manifest.json")) ??
                    throw new InvalidDataException("Failed to read manifest.json");

                if (!_serializers.TryGetValue(manifest.DataFormat, out var serializer))
                    throw new NotSupportedException($"Unknown data format: [{manifest.DataFormat}]");

                List<Game> games = serializer.Deserialize(ImportTools.ReadEntry(archive, manifest.DataFile));

                Dictionary<int, string> coverPaths = archive.Entries.Where(e => e.FullName.StartsWith($"{manifest.CoversDirectory}/") && !e.FullName.EndsWith("/"))
                    .ToDictionary(
                        e => int.Parse(Path.GetFileNameWithoutExtension(e.FullName)),
                        e => e.FullName);
                var package = new GCATPackage
                {
                    Games = games,
                    CoverPaths = coverPaths
                };

                return (manifest, package);
            }, token);
        }
    }
}
