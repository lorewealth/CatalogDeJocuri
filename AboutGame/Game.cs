using AboutGame.Enums;

namespace AboutGame
{
    public class Game
    {
        //variables
        public int InternalId { get; set; } 
        public string? ExternalId { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public List<Genre> Genres { get; set; } = [];
        public string GenresStr => string.Join(", ", Genres.Select(gen => gen.Name));
        public List<Publisher> Publishers { get; set; } = [];
        public string PublishersStr => string.Join(", ", Publishers.Select(pub => pub.Name));
        public List<Developer> Developers { get; set; } = [];
        public string DevelopersStr => string.Join(", ", Developers.Select(dev => dev.Name));
        public List<AgeRating> AgeRatings { get; set; } = [];
        public string AgeRatingsStr => string.Join(", ", AgeRatings.Select(val => val.AgeRatingValueStr));
        public List<Rating> Scores { get; set; } = [];
        public string ScoresStr => string.Join(", ", Scores.Select(score => $"{score.Source}: {score.NormalizeScore:F1}/10"));
        public AvailableStores Stores { get; set; }
        public SupportedOS SupportedOS { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImgUrl { get; set; }
    }
}
