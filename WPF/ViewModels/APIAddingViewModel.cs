using AboutGame;
using APISearch;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Input;
using WPF.Stores;
using WPF.Tools;
using WPF.Models;
using WPF.Validators;

namespace WPF.ViewModels;

public class APIAddingViewModel : BaseViewModel
{
    private readonly ManagingCache _cache;
    private readonly GameStore _gameStore;
    private string _searchText = "";
    private string _country = "US";
    private GameSearchResult? _selectedResult;
    private Game? _selectedGame;
    private string? _errorMessage;
    private string? _successMessage;
    private bool _isSearching;
    private bool _isLoadingDetails;
    private bool _isAdding;

    public ObservableCollection<GameSearchResult> Results { get; } = [];
    public ObservableCollection<AgeRatingDisplay> AgeRatings { get; } = [];
    public IReadOnlyList<string> Countries { get; } = ["US", "GB", "DE", "FR", "UA"];

    public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }
    public string Country { get => _country; set => SetProperty(ref _country, value); }
    public GameSearchResult? SelectedResult { get => _selectedResult; set => SetProperty(ref _selectedResult, value); }
    public Game? SelectedGame
    {
        get => _selectedGame;
        private set
        {
            if (SetProperty(ref _selectedGame, value))
            {
                RefreshAgeRatings();
                OnPropertyChanged(nameof(HasSelectedGame));
            }
        }
    }
    public bool IsSearching { get => _isSearching; private set => SetProperty(ref _isSearching, value); }
    public bool IsLoadingDetails { get => _isLoadingDetails; private set => SetProperty(ref _isLoadingDetails, value); }
    public bool IsAdding { get => _isAdding; private set => SetProperty(ref _isAdding, value); }
    public bool IsBusy => IsSearching || IsLoadingDetails || IsAdding;
    public bool HasSelectedGame => SelectedGame is not null;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool HasSuccess => !string.IsNullOrWhiteSpace(SuccessMessage);

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set { if (SetProperty(ref _errorMessage, value)) OnPropertyChanged(nameof(HasError)); }
    }
    public string? SuccessMessage
    {
        get => _successMessage;
        private set { if (SetProperty(ref _successMessage, value)) OnPropertyChanged(nameof(HasSuccess)); }
    }

    public ICommand SearchCommand { get; }
    public ICommand LoadDetailsCommand { get; }
    public ICommand AddGameCommand { get; }

    public APIAddingViewModel(ManagingCache cache, GameStore gameStore)
    {
        _cache = cache;
        _gameStore = gameStore;
        SearchCommand = new RelayCommand(async _ => await SearchAsync(), _ => !IsBusy);
        LoadDetailsCommand = new RelayCommand(async _ => await LoadDetailsAsync(), _ => SelectedResult is not null && !IsBusy);
        AddGameCommand = new RelayCommand(async _ => await AddGameAsync(), _ => SelectedGame is not null && !IsBusy);
    }

    private async Task SearchAsync()
    {
        ClearMessages();
        Results.Clear();
        SelectedGame = null;
        if (!OnlineSearchValidator.TryValidateSearchText(SearchText, out string? error))
        {
            ErrorMessage = error;
            return;
        }

        SetBusy(ref _isSearching, true, nameof(IsSearching));
        try
        {
            GameSearchResult[] results = await APIService.SearchAsync(SearchText.Trim()) ?? [];
            foreach (GameSearchResult result in results)
                Results.Add(result);
            if (Results.Count == 0)
                ErrorMessage = "No games were found.";
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Could not reach the game service. Check your internet connection and try again.";
        }
        catch (JsonException)
        {
            ErrorMessage = "The game service returned an unexpected response.";
        }
        finally { SetBusy(ref _isSearching, false, nameof(IsSearching)); }
    }

    private async Task LoadDetailsAsync()
    {
        if (SelectedResult is null)
            return;
        ClearMessages();
        string cacheKey = $"{SelectedResult.IGDBId}:{Country}";
        if (_cache.TryGet(cacheKey, out Game? cached))
        {
            SelectedGame = cached;
            return;
        }

        SetBusy(ref _isLoadingDetails, true, nameof(IsLoadingDetails));
        try
        {
            SelectedGame = await APIService.RetrieveAsync(SelectedResult.IGDBId, Country);
            if (SelectedGame is null)
            {
                ErrorMessage = "Details for this game could not be found.";
                return;
            }
            _cache.Add(cacheKey, SelectedGame);
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Could not load game details. Check your internet connection and try again.";
        }
        catch (JsonException)
        {
            ErrorMessage = "The game service returned invalid game details.";
        }
        finally { SetBusy(ref _isLoadingDetails, false, nameof(IsLoadingDetails)); }
    }

    private async Task AddGameAsync()
    {
        if (SelectedGame is null)
            return;
        ClearMessages();

        if (OnlineSearchValidator.IsAlreadyInCatalog(SelectedGame, _gameStore.Games))
        {
            ErrorMessage = $"{SelectedGame.Name} is already in your catalog.";
            return;
        }

        SetBusy(ref _isAdding, true, nameof(IsAdding));
        try
        {
            if (!await _gameStore.AddGameAsync(SelectedGame))
            {
                ErrorMessage = $"This game could not be added: {_gameStore.LastError ?? "unknown database error"}";
                return;
            }
            SuccessMessage = $"{SelectedGame.Name} was added to your catalog.";
        }
        finally { SetBusy(ref _isAdding, false, nameof(IsAdding)); }
    }

    private void ClearMessages() { ErrorMessage = null; SuccessMessage = null; }

    private void RefreshAgeRatings()
    {
        AgeRatings.Clear();
        if (SelectedGame is null)
            return;

        foreach (AgeRating rating in SelectedGame.AgeRatings)
            AgeRatings.Add(new AgeRatingDisplay(rating));
    }

    private void SetBusy(ref bool field, bool value, string propertyName)
    {
        if (SetProperty(ref field, value, propertyName))
            OnPropertyChanged(nameof(IsBusy));
    }
}
