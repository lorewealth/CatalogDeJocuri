using System.Collections.Specialized;
using WPF.Stores;

namespace WPF.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private readonly GameStore _gameStore;

    public int TotalGames => _gameStore.Games.Count;

    public HomeViewModel(GameStore gameStore)
    {
        _gameStore = gameStore;
        _gameStore.Games.CollectionChanged += OnGamesChanged;
    }

    private void OnGamesChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs) =>
        OnPropertyChanged(nameof(TotalGames));
}
