using System.IO.Compression;

namespace Stocking.Tools
{
    internal class ExportTools
    {
        internal static void WriteEntry(ZipArchive archive, string entryName, byte[] bytes)
        {
            ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
            using var stream = entry.Open();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
