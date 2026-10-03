namespace Stocking.Packaging
{
    public sealed class GCATManifest
    {
        public int FormatVersion { get; init; } = 1;
        public string DataFormat { get; init; } = string.Empty;
        public string DataFile { get; init; } = string.Empty;
        public string CoversDirectory { get; init; } = "covers";
    }
}
