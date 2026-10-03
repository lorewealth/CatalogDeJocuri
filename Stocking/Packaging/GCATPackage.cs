using AboutGame;

namespace Stocking.Packaging
{
    public sealed class GCATPackage
    {
        public required List<Game> Games{ get; init; }
        public Dictionary<int, string> CoverPaths { get; init; } = [];
    }
}
