namespace AboutGame
{
    public class Publisher(int id, string name) : IIdentifiable
    {
        public int Id { get; } = id;
        public string Name { get; } = name;
    }
}