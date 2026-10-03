using AboutGame;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocking.Tools
{
    internal class ImportTools
    {
        internal static byte[] ReadEntry(ZipArchive archive, string entryName)
        {
            ZipArchiveEntry entry = archive.GetEntry(entryName) ??
                throw new InvalidDataException($"Entry not found: [{entryName}]");
            using var stream = entry.Open();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
