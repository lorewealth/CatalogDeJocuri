using AboutGame;
using System.Collections.ObjectModel;
using WPF.Stores;

namespace WPF.ViewModels
{
    public class GamesViewModel(GameStore gameStore) : BaseViewModel
    {
        private readonly GameStore gameStore = gameStore;
        public ObservableCollection<Game> Games => gameStore.Games;
        private Game? _CurrentGame;
        public Game? CurrentGame
        {
            get => _CurrentGame;
            set
            {
                _CurrentGame = value;
                OnPropertyChanged();
            }
        }
        public Task GetGamesAsync() => gameStore.LoadAsync();
    }
}
