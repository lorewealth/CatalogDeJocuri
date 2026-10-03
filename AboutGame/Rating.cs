using AboutGame.Enums;

namespace AboutGame
{
    public record Rating(RatingSource Source, double RawScore, string Url="none", int Count = 0)
    {
        public RatingSource Source { get; init; } = Source;
        public string Url { get; init; } = Url;
        public int Count { get; init; } = Count;
        public double RawScore { get; init; } = RawScore;
        public const double MinScore = 0;
        public double MaxScore => (double)Source;
        public double NormalizeScore 
        { 
            get 
            { 
                double safeScore = Math.Clamp(RawScore, MinScore, MaxScore);
                return MaxScore > MinScore ? ((safeScore - MinScore) / (MaxScore - MinScore)) * 10 : MinScore; 
            } 
        }
    }
}
