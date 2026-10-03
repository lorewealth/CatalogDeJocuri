using AboutGame.Enums;

namespace AboutGame
{
    public record AgeRating(AgeRatingsCategory Source, AgeRatingsValue Age)
    {
        public AgeRatingsCategory Source { get; set; } = Source;
        public AgeRatingsValue Age { get; set; } = Age;
        public string? AgeRatingValueStr => Enum.GetName<AgeRatingsValue>(Age);
    }
}
