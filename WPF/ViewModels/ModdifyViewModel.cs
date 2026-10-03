using AboutGame;
using AboutGame.Enums;
using System.Windows.Input;
using WPF.Stores;
using WPF.Tools;
using WPF.Validators;

namespace WPF.ViewModels;

public class ModdifyViewModel : BaseViewModel
{
    private readonly GameStore _gameStore;
    private Game? _selectedGame;
    private string _name = "";
    private string _priceText = "";
    private string _genresText = "";
    private string _publishersText = "";
    private string _developersText = "";
    private DateTime? _releaseDate;
    private bool _isAvailable;
    private string? _errorMessage;
    private string? _successMessage;
    private bool _isSaving;

    public IEnumerable<Game> Games => _gameStore.Games;
    public List<ComboBoxFilter<AvailableStores>> StoreOptions { get; }
    public List<ComboBoxFilter<SupportedOS>> OperatingSystemOptions { get; }

    public Game? SelectedGame
    {
        get => _selectedGame;
        set
        {
            if (SetProperty(ref _selectedGame, value))
                LoadSelectedGame();
        }
    }
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string PriceText { get => _priceText; set => SetProperty(ref _priceText, value); }
    public string GenresText { get => _genresText; set => SetProperty(ref _genresText, value); }
    public string PublishersText { get => _publishersText; set => SetProperty(ref _publishersText, value); }
    public string DevelopersText { get => _developersText; set => SetProperty(ref _developersText, value); }
    public DateTime? ReleaseDate { get => _releaseDate; set => SetProperty(ref _releaseDate, value); }
    public bool IsAvailable { get => _isAvailable; set => SetProperty(ref _isAvailable, value); }
    public bool HasSelection => SelectedGame is not null;
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

    public ICommand SaveCommand { get; }

    public ModdifyViewModel(GameStore gameStore)
    {
        _gameStore = gameStore;
        StoreOptions = [.. Enum.GetValues<AvailableStores>().Where(value => value is not AvailableStores.None and not AvailableStores.All).Select(value => new ComboBoxFilter<AvailableStores>(value))];
        OperatingSystemOptions = [.. Enum.GetValues<SupportedOS>().Where(value => value is not SupportedOS.None and not SupportedOS.All).Select(value => new ComboBoxFilter<SupportedOS>(value))];
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => SelectedGame is not null && !IsSaving);
    }

    private void LoadSelectedGame()
    {
        ErrorMessage = null;
        SuccessMessage = null;
        if (SelectedGame is null)
        {
            Name = PriceText = GenresText = PublishersText = DevelopersText = "";
            ReleaseDate = null;
            return;
        }

        Name = SelectedGame.Name;
        PriceText = SelectedGame.Price.ToString(System.Globalization.CultureInfo.CurrentCulture);
        GenresText = string.Join(", ", SelectedGame.Genres.Select(item => item.Name));
        PublishersText = string.Join(", ", SelectedGame.Publishers.Select(item => item.Name));
        DevelopersText = string.Join(", ", SelectedGame.Developers.Select(item => item.Name));
        ReleaseDate = SelectedGame.ReleaseDate;
        IsAvailable = SelectedGame.IsAvailable;
        foreach (ComboBoxFilter<AvailableStores> option in StoreOptions) option.IsSelected = SelectedGame.Stores.HasFlag(option.Value);
        foreach (ComboBoxFilter<SupportedOS> option in OperatingSystemOptions) option.IsSelected = SelectedGame.SupportedOS.HasFlag(option.Value);
        OnPropertyChanged(nameof(HasSelection));
    }

    private async Task SaveAsync()
    {
        if (SelectedGame is null)
            return;
        ErrorMessage = null;
        SuccessMessage = null;

        if (!GameInputValidator.TryParseGameDetails(Name, PriceText, ReleaseDate, GenresText, PublishersText, DevelopersText,
                out GameInputValidator.GameInput input, out string? error))
        {
            ErrorMessage = error;
            return;
        }
        AvailableStores stores = StoreOptions.Where(option => option.IsSelected).Aggregate(AvailableStores.None, (value, option) => value | option.Value);
        SupportedOS operatingSystems = OperatingSystemOptions.Where(option => option.IsSelected).Aggregate(SupportedOS.None, (value, option) => value | option.Value);
        if (!GameInputValidator.TryValidatePlatforms(stores != AvailableStores.None, operatingSystems != SupportedOS.None, out error))
        {
            ErrorMessage = error;
            return;
        }

        Game updated = new()
        {
            InternalId = SelectedGame.InternalId, ExternalId = SelectedGame.ExternalId, Name = input.Name, Price = input.Price,
            Genres = input.Genres.Select(value => new Genre(0, value)).ToList(), Publishers = input.Publishers.Select(value => new Publisher(0, value)).ToList(), Developers = input.Developers.Select(value => new Developer(0, value)).ToList(), Stores = stores, SupportedOS = operatingSystems,
            ReleaseDate = input.ReleaseDate, IsAvailable = IsAvailable, ImgUrl = SelectedGame.ImgUrl,
            Scores = [.. SelectedGame.Scores], AgeRatings = [.. SelectedGame.AgeRatings]
        };

        IsSaving = true;
        try
        {
            if (!await _gameStore.UpdateGameAsync(updated))
            {
                ErrorMessage = $"Could not save changes: {_gameStore.LastError ?? "unknown database error"}";
                return;
            }
            SelectedGame = _gameStore.Games.FirstOrDefault(game => game.InternalId == updated.InternalId);
            SuccessMessage = $"{updated.Name} was updated.";
            OnPropertyChanged(nameof(Games));
        }
        finally { IsSaving = false; }
    }

}
