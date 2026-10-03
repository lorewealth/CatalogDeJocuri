using AboutGame;
using Stocking.Serialization;
using System.IO.Compression;
using Stocking.Tools;
using System.Text.Json;

namespace Stocking.Packaging
{
    public class GCATExporter
    {
        public async Task ExportAsync(GCATPackage package, ISerializer<List<Game>> gamesSerializer, string destDir, CancellationToken token = default)
        {
            if (Path.GetExtension(destDir) != ".gcat")
                destDir = Path.ChangeExtension(destDir, ".gcat");

            if (File.Exists(destDir)) File.Delete(destDir);

            await Task.Run(() =>
            {
                GCATManifest manifest = new()
                {
                    FormatVersion = 1,
                    DataFormat = gamesSerializer.FileExtension,
                    DataFile = $"data.{gamesSerializer.FileExtension}",
                    CoversDirectory = "covers"
                };

                using ZipArchive archive = ZipFile.Open(destDir, ZipArchiveMode.Create);
                
                byte[] manifestBytes = JsonSerializer.SerializeToUtf8Bytes(manifest);
                ExportTools.WriteEntry(archive, "manifest.json", manifestBytes);

                byte[] dataBytes = gamesSerializer.Serialize(package.Games);
                ExportTools.WriteEntry(archive, manifest.DataFile, dataBytes);

                foreach (var (internalId, sourcePath) in package.CoverPaths)
                {
                    token.ThrowIfCancellationRequested();
                    string ext = Path.GetExtension(sourcePath);
                    string entryName = $"{manifest.CoversDirectory}/{internalId}{ext}";
                    archive.CreateEntryFromFile(sourcePath, entryName, CompressionLevel.Optimal);
                }
            }, token);
        }
    }
}
