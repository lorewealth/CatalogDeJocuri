using AboutGame;
using AboutGame.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WPF.Stores;
using WPF.Tools;
using WPF.Validators;

namespace WPF.ViewModels;

public class ManualAddingViewModel : BaseViewModel
{
    private readonly GameStore _gameStore;
    private string _name = "";
    private string _priceText = "";
    private string _genresText = "";
    private string _publishersText = "";
    private string _developersText = "";
    private string _scoreText = "";
    private DateTime? _releaseDate = DateTime.Today;
    private bool _isAvailable = true;
    private AgeRatingsValue _selectedAgeRating = AgeRatingsValue.PEGI3;
    private string? _errorMessage;
    private string? _successMessage;
    private bool _isSaving;

    public ObservableCollection<Publisher> Publishers { get; }
    public ObservableCollection<Developer> Developers { get; }
    public List<ComboBoxFilter<AvailableStores>> StoreOptions { get; }
    public List<ComboBoxFilter<SupportedOS>> OperatingSystemOptions { get; }
    public List<AgeRatingsValue> AgeRatingOptions { get; } = [.. Enum.GetValues<AgeRatingsValue>()];

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string PriceText { get => _priceText; set => SetProperty(ref _priceText, value); }
    public string GenresText { get => _genresText; set => SetProperty(ref _genresText, value); }
    public string PublishersText { get => _publishersText; set => SetProperty(ref _publishersText, value); }
    public string DevelopersText { get => _developersText; set => SetProperty(ref _developersText, value); }
    public string ScoreText { get => _scoreText; set => SetProperty(ref _scoreText, value); }
    public DateTime? ReleaseDate { get => _releaseDate; set => SetProperty(ref _releaseDate, value); }
    public bool IsAvailable { get => _isAvailable; set => SetProperty(ref _isAvailable, value); }
    public AgeRatingsValue SelectedAgeRating { get => _selectedAgeRating; set => SetProperty(ref _selectedAgeRating, value); }
    public bool IsSaving { get => _isSaving; private set => SetProperty(ref _isSaving, value); }
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

    public ICommand AddGameCommand { get; }

    public ManualAddingViewModel(GameStore gameStore)
    {
        _gameStore = gameStore;
        Publishers = gameStore.PublishersLST;
        Developers = gameStore.DevelopersLST;
        StoreOptions = [.. Enum.GetValues<AvailableStores>()
            .Where(value => value is not AvailableStores.None and not AvailableStores.All)
            .Select(value => new ComboBoxFilter<AvailableStores>(value))];
        OperatingSystemOptions = [.. Enum.GetValues<SupportedOS>()
            .Where(value => value is not SupportedOS.None and not SupportedOS.All)
            .Select(value => new ComboBoxFilter<SupportedOS>(value))];

        AddGameCommand = new RelayCommand(async _ => await AddGameAsync(), _ => !IsSaving);
    }

    private async Task AddGameAsync()
    {
        ErrorMessage = null;
        SuccessMessage = null;

        if (!GameInputValidator.TryParseGameDetails(Name, PriceText, ReleaseDate, GenresText, PublishersText, DevelopersText,
                out GameInputValidator.GameInput input, out string? error))
        {
            ErrorMessage = error;
            return;
        }

        AvailableStores stores = StoreOptions.Where(option => option.IsSelected)
            .Aggregate(AvailableStores.None, (result, option) => result | option.Value);
        SupportedOS operatingSystems = OperatingSystemOptions.Where(option => option.IsSelected)
            .Aggregate(SupportedOS.None, (result, option) => result | option.Value);
        if (!GameInputValidator.TryValidatePlatforms(stores != AvailableStores.None, operatingSystems != SupportedOS.None, out error))
        {
            ErrorMessage = error;
            return;
        }

        List<Rating> scores = [];
        if (!GameInputValidator.TryParseOptionalScore(ScoreText, out double? score, out error))
        {
            ErrorMessage = error;
            return;
        }
        if (score is not null) scores.Add(new Rating(RatingSource.IGDB, score.Value));

        Game game = new()
        {
            Name = input.Name, Price = input.Price, Genres = input.Genres.Select(name => new Genre(0, name)).ToList(),
            Publishers = input.Publishers.Select(name => new Publisher(0, name)).ToList(), Developers = input.Developers.Select(name => new Developer(0, name)).ToList(),
            Stores = stores, SupportedOS = operatingSystems, ReleaseDate = input.ReleaseDate,
            IsAvailable = IsAvailable, Scores = scores,
            AgeRatings = [new AgeRating(AgeRatingsCategory.PEGI, SelectedAgeRating)]
        };

        IsSaving = true;
        try
        {
            if (!await _gameStore.AddGameAsync(game))
            {
                ErrorMessage = $"The game could not be saved: {_gameStore.LastError ?? "unknown database error"}";
                return;
            }
            ResetForm();
            SuccessMessage = "Game added successfully.";
        }
        finally { IsSaving = false; }
    }

    private void ResetForm()
    {
        Name = PriceText = GenresText = PublishersText = DevelopersText = ScoreText = "";
        ReleaseDate = DateTime.Today;
        IsAvailable = true;
        SelectedAgeRating = AgeRatingsValue.PEGI3;
        foreach (var option in StoreOptions) option.IsSelected = false;
        foreach (var option in OperatingSystemOptions) option.IsSelected = false;
    }
}
