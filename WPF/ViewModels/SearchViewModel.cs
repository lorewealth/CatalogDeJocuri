using AboutGame;
using AboutGame.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WPF.Stores;
using WPF.Tools;
using WPF.Validators;

namespace WPF.ViewModels;

public class SearchViewModel : BaseViewModel
{
    private readonly GameStore _gameStore;
    private string? _errorMessage;
    private string _nameQueryBox = "";
    private string _priceQueryBox = "";
    private string _scoreQueryBox = "";
    private string _genreQueryBox = "";
    private string _publisherQueryBox = "";
    private string _developerQueryBox = "";
    private string _dateQueryBox = "";
    private Game? _selectedGame;

    public ICommand InternalSearchCommand { get; }
    public ICommand ShowAllGamesCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ObservableCollection<Game> Results { get; } = [];

    public string NameQueryBox { get => _nameQueryBox; set => SetProperty(ref _nameQueryBox, value); }
    public string PriceQueryBox { get => _priceQueryBox; set => SetProperty(ref _priceQueryBox, value); }
    public string ScoreQueryBox { get => _scoreQueryBox; set => SetProperty(ref _scoreQueryBox, value); }
    public string GenreQueryBox { get => _genreQueryBox; set => SetProperty(ref _genreQueryBox, value); }
    public string PublisherQueryBox { get => _publisherQueryBox; set => SetProperty(ref _publisherQueryBox, value); }
    public string DeveloperQueryBox { get => _developerQueryBox; set => SetProperty(ref _developerQueryBox, value); }
    public string DateQueryBox { get => _dateQueryBox; set => SetProperty(ref _dateQueryBox, value); }
    public Game? SelectedGame { get => _selectedGame; set => SetProperty(ref _selectedGame, value); }

    public List<ComboBoxFilter<AgeRatingsValue>> AgeOptions { get; }
    public List<ComboBoxFilter<SupportedOS>> OSOptions { get; }
    public List<ComboBoxFilter<AvailableStores>> StoreOptions { get; }

    private bool _searchByName = true;
    private bool _searchByPrice;
    private bool _searchByScore;
    private bool _searchByGenre;
    private bool _searchByPublisher;
    private bool _searchByDeveloper;
    private bool _searchByAgeRate;
    private bool _searchByStore;
    private bool _searchByDate;
    private bool _searchByOS;

    public bool SearchByName { get => _searchByName; set => SetProperty(ref _searchByName, value); }
    public bool SearchByPrice { get => _searchByPrice; set => SetProperty(ref _searchByPrice, value); }
    public bool SearchByScore { get => _searchByScore; set => SetProperty(ref _searchByScore, value); }
    public bool SearchByGenre { get => _searchByGenre; set => SetProperty(ref _searchByGenre, value); }
    public bool SearchByPublisher { get => _searchByPublisher; set => SetProperty(ref _searchByPublisher, value); }
    public bool SearchByDeveloper { get => _searchByDeveloper; set => SetProperty(ref _searchByDeveloper, value); }
    public bool SearchByAgeRate { get => _searchByAgeRate; set => SetProperty(ref _searchByAgeRate, value); }
    public bool SearchByStore { get => _searchByStore; set => SetProperty(ref _searchByStore, value); }
    public bool SearchByDate { get => _searchByDate; set => SetProperty(ref _searchByDate, value); }
    public bool SearchByOS { get => _searchByOS; set => SetProperty(ref _searchByOS, value); }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
                OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool HasResults => Results.Count > 0;
    public string ResultsSummary => Results.Count == 1 ? "1 game found" : $"{Results.Count} games found";

    public SearchViewModel(GameStore gameStore)
    {
        _gameStore = gameStore;

        AgeOptions = [.. Enum.GetValues<AgeRatingsValue>()
            .Select(value => new ComboBoxFilter<AgeRatingsValue>(value))];
        OSOptions = [.. Enum.GetValues<SupportedOS>()
            .Where(value => value is not SupportedOS.None and not SupportedOS.All)
            .Select(value => new ComboBoxFilter<SupportedOS>(value))];
        StoreOptions = [.. Enum.GetValues<AvailableStores>()
            .Where(value => value is not AvailableStores.None and not AvailableStores.All)
            .Select(value => new ComboBoxFilter<AvailableStores>(value))];

        InternalSearchCommand = new RelayCommand(_ => Search());
        ShowAllGamesCommand = new RelayCommand(_ => ShowAllGames());
        ClearSearchCommand = new RelayCommand(_ => ClearSearch());
    }

    private void Search()
    {
        Results.Clear();
        SelectedGame = null;
        OnPropertyChanged(nameof(HasResults));
        OnPropertyChanged(nameof(ResultsSummary));
        ErrorMessage = null;

        if (!TryBuildFilters(out SearchFilters filters, out string? error))
        {
            ErrorMessage = error;
            return;
        }

        IEnumerable<Game> games = _gameStore.Games;

        if (filters.Name is not null)
            games = games.Where(game => Contains(game.Name, filters.Name));
        if (filters.Price is { } price)
            games = games.Where(game => game.Price >= price.Min && game.Price <= price.Max);
        if (filters.Score is { } score)
            games = games.Where(game => game.Scores.Any(rating =>
                (decimal)rating.NormalizeScore >= score.Min && (decimal)rating.NormalizeScore <= score.Max));
        if (filters.Genres.Length > 0)
            games = games.Where(game => game.Genres.Any(genre => filters.Genres.Any(term => Contains(genre.Name, term))));
        if (filters.Publishers.Length > 0)
            games = games.Where(game => game.Publishers.Any(publisher => filters.Publishers.Any(term => Contains(publisher.Name, term))));
        if (filters.Developers.Length > 0)
            games = games.Where(game => game.Developers.Any(developer => filters.Developers.Any(term => Contains(developer.Name, term))));
        if (filters.Ages.Length > 0)
            games = games.Where(game => game.AgeRatings.Any(rating => filters.Ages.Contains(rating.Age)));
        if (filters.Stores.Length > 0)
            games = games.Where(game => filters.Stores.Any(store => game.Stores.HasFlag(store)));
        if (filters.OperatingSystems.Length > 0)
            games = games.Where(game => filters.OperatingSystems.All(os => game.SupportedOS.HasFlag(os)));
        if (filters.Years is { } years)
            games = games.Where(game => game.ReleaseDate.Year >= years.Min && game.ReleaseDate.Year <= years.Max);

        foreach (Game game in games)
            Results.Add(game);

        if (Results.Count == 0)
            ErrorMessage = "No games found.";

        OnPropertyChanged(nameof(HasResults));
        OnPropertyChanged(nameof(ResultsSummary));
    }

    private void ShowAllGames()
    {
        Results.Clear();
        SelectedGame = null;
        ErrorMessage = null;

        foreach (Game game in _gameStore.Games)
            Results.Add(game);

        if (Results.Count == 0)
            ErrorMessage = "Your catalog has no games yet.";

        OnPropertyChanged(nameof(HasResults));
        OnPropertyChanged(nameof(ResultsSummary));
    }

    private void ClearSearch()
    {
        NameQueryBox = PriceQueryBox = ScoreQueryBox = GenreQueryBox = PublisherQueryBox = DeveloperQueryBox = DateQueryBox = "";
        SearchByName = true;
        SearchByPrice = SearchByScore = SearchByGenre = SearchByPublisher = SearchByDeveloper = false;
        SearchByAgeRate = SearchByStore = SearchByDate = SearchByOS = false;
        foreach (ComboBoxFilter<AgeRatingsValue> option in AgeOptions) option.IsSelected = false;
        foreach (ComboBoxFilter<AvailableStores> option in StoreOptions) option.IsSelected = false;
        foreach (ComboBoxFilter<SupportedOS> option in OSOptions) option.IsSelected = false;
        Results.Clear();
        SelectedGame = null;
        ErrorMessage = null;
        OnPropertyChanged(nameof(HasResults));
        OnPropertyChanged(nameof(ResultsSummary));
    }

    private bool TryBuildFilters(out SearchFilters filters, out string? error)
    {
        filters = new SearchFilters();
        error = null;

        if (SearchByName)
        {
            if (!SearchValidator.TryGetRequiredText(NameQueryBox, "name", out string? name, out error)) return false;
            filters.Name = name;
        }
        if (SearchByGenre)
        {
            if (!SearchValidator.TryGetTerms(GenreQueryBox, "genre", out string[] genres, out error)) return false;
            filters.Genres = genres;
        }
        if (SearchByPublisher)
        {
            if (!SearchValidator.TryGetTerms(PublisherQueryBox, "publisher", out string[] publishers, out error)) return false;
            filters.Publishers = publishers;
        }
        if (SearchByDeveloper)
        {
            if (!SearchValidator.TryGetTerms(DeveloperQueryBox, "developer", out string[] developers, out error)) return false;
            filters.Developers = developers;
        }
        if (SearchByPrice)
        {
            if (!SearchValidator.TryGetRange(PriceQueryBox, "price", 0m, decimal.MaxValue, out SearchValidator.DecimalRange? price, out error)) return false;
            filters.Price = price;
        }
        if (SearchByScore)
        {
            if (!SearchValidator.TryGetRange(ScoreQueryBox, "score", 0m, 10m, out SearchValidator.DecimalRange? score, out error)) return false;
            filters.Score = score;
        }
        if (SearchByDate)
        {
            if (!SearchValidator.TryGetYearRange(DateQueryBox, out SearchValidator.IntRange? years, out error)) return false;
            filters.Years = years;
        }

        filters.Ages = SearchByAgeRate
            ? AgeOptions.Where(option => option.IsSelected).Select(option => option.Value).ToArray()
            : [];
        filters.Stores = SearchByStore
            ? StoreOptions.Where(option => option.IsSelected).Select(option => option.Value).ToArray()
            : [];
        filters.OperatingSystems = SearchByOS
            ? OSOptions.Where(option => option.IsSelected).Select(option => option.Value).ToArray()
            : [];

        if (SearchByAgeRate && filters.Ages.Length == 0) { error = "Select at least one age rating."; return false; }
        if (SearchByStore && filters.Stores.Length == 0) { error = "Select at least one store."; return false; }
        if (SearchByOS && filters.OperatingSystems.Length == 0) { error = "Select at least one supported operating system."; return false; }

        return true;
    }

    private static bool Contains(string value, string query) =>
        value.Contains(query, StringComparison.OrdinalIgnoreCase);

    private sealed class SearchFilters
    {
        public string? Name { get; set; }
        public string[] Genres { get; set; } = [];
        public string[] Publishers { get; set; } = [];
        public string[] Developers { get; set; } = [];
        public SearchValidator.DecimalRange? Price { get; set; }
        public SearchValidator.DecimalRange? Score { get; set; }
        public SearchValidator.IntRange? Years { get; set; }
        public AgeRatingsValue[] Ages { get; set; } = [];
        public AvailableStores[] Stores { get; set; } = [];
        public SupportedOS[] OperatingSystems { get; set; } = [];
    }

}
