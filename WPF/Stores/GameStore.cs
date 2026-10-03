using AboutGame;
using Stocking.Database;
using System.Collections.ObjectModel;

namespace WPF.Stores
{
    public sealed class GameStore(SQLStocking rep)
    {
        public readonly SQLStocking rep = rep;
        public string? LastError => rep.LastError;
        public ObservableCollection<Game> Games { get; } = [];
        public ObservableCollection<Publisher> PublishersLST { get; } = [];
        public ObservableCollection<Developer> DevelopersLST { get; } = [];

        public async Task LoadAsync()
        {
            IEnumerable<Game> games = await rep.GetGames();
            Games.Clear();

            foreach(Game game in games)
                Games.Add(game);

            PublishersLST.Clear();
            foreach (Publisher publisher in Games.SelectMany(game => game.Publishers)
                         .DistinctBy(publisher => publisher.Name, StringComparer.OrdinalIgnoreCase)
                         .OrderBy(publisher => publisher.Name))
                PublishersLST.Add(publisher);

            DevelopersLST.Clear();
            foreach (Developer developer in Games.SelectMany(game => game.Developers)
                         .DistinctBy(developer => developer.Name, StringComparer.OrdinalIgnoreCase)
                         .OrderBy(developer => developer.Name))
                DevelopersLST.Add(developer);
        }

        public async Task<bool> AddGameAsync(Game game)
        {
            if (!await rep.AddGame(game))
                return false;

            await LoadAsync();
            return true;
        }

        public async Task<bool> UpdateGameAsync(Game game)
        {
            if (!await rep.UpdateGame(game))
                return false;

            await LoadAsync();
            return true;
        }

        public async Task<(int Added, int Skipped, int Failed)> ImportGamesAsync(IEnumerable<Game> games)
        {
            int added = 0;
            int skipped = 0;
            int failed = 0;
            HashSet<string> knownExternalIds = Games
                .Where(game => !string.IsNullOrWhiteSpace(game.ExternalId))
                .Select(game => game.ExternalId!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (Game game in games)
            {
                game.InternalId = 0;
                if (!string.IsNullOrWhiteSpace(game.ExternalId) && !knownExternalIds.Add(game.ExternalId))
                {
                    skipped++;
                    continue;
                }

                if (await rep.AddGame(game))
                    added++;
                else
                    failed++;
            }

            if (added > 0)
                await LoadAsync();
            return (added, skipped, failed);
        }
    }
}
