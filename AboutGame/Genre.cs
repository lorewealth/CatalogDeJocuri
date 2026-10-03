namespace AboutGame
{
    public class Genre(int id, string name) : IIdentifiable
    {
        public int Id { get; } = id;
        public string Name { get; } = name;
    }
}
