using AboutGame;
using Microsoft.Win32;
using Stocking.Packaging;
using Stocking.Serialization;
using System.IO;
using System.Windows.Input;
using WPF.Stores;
using WPF.Tools;

namespace WPF.ViewModels;

public class ImportExportViewModel : BaseViewModel
{
    private readonly GameStore _gameStore;
    private string _selectedFormat = "JSON";
    private string? _errorMessage;
    private string? _successMessage;
    private bool _isBusy;

    public IReadOnlyList<string> ExportFormats { get; } = ["JSON", "TXT", "CSV", "BIN"];
    public string SelectedFormat { get => _selectedFormat; set => SetProperty(ref _selectedFormat, value); }
    public bool IsBusy { get => _isBusy; private set => SetProperty(ref _isBusy, value); }
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

    public ICommand ExportCommand { get; }
    public ICommand ImportCommand { get; }

    public ImportExportViewModel(GameStore gameStore)
    {
        _gameStore = gameStore;
        ExportCommand = new RelayCommand(async _ => await ExportAsync(), _ => !IsBusy);
        ImportCommand = new RelayCommand(async _ => await ImportAsync(), _ => !IsBusy);
    }

    private async Task ExportAsync()
    {
        ClearMessages();
        if (_gameStore.Games.Count == 0)
        {
            ErrorMessage = "There are no games to export.";
            return;
        }

        SaveFileDialog dialog = new()
        {
            Title = "Export Game Catalog",
            Filter = "Game Catalog archive (*.gcat)|*.gcat",
            DefaultExt = ".gcat",
            AddExtension = true,
            FileName = "GameCatalog.gcat"
        };
        if (dialog.ShowDialog() != true)
            return;

        IsBusy = true;
        try
        {
            ISerializer<List<Game>> serializer = CreateSerializer();
            GCATPackage package = new() { Games = [.. _gameStore.Games] };
            await new GCATExporter().ExportAsync(package, serializer, dialog.FileName);
            SuccessMessage = $"Exported {_gameStore.Games.Count} game(s) to {Path.GetFileName(dialog.FileName)}.";
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Export failed: {exception.Message}";
        }
        finally { IsBusy = false; }
    }

    private async Task ImportAsync()
    {
        ClearMessages();
        OpenFileDialog dialog = new()
        {
            Title = "Import Game Catalog",
            Filter = "Game Catalog archive (*.gcat)|*.gcat"
        };
        if (dialog.ShowDialog() != true)
            return;

        IsBusy = true;
        try
        {
            ISerializer<List<Game>>[] serializers = [new TXTSerializer(), new JSONSerializer(), new CSVSerializer(), new BINSerializer()];
            (_, GCATPackage package) = await new GCATImporter(serializers).ImportAsync(dialog.FileName);
            (int added, int skipped, int failed) = await _gameStore.ImportGamesAsync(package.Games);
            SuccessMessage = $"Import complete: {added} added, {skipped} already present, {failed} failed.";
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Import failed: {exception.Message}";
        }
        finally { IsBusy = false; }
    }

    private ISerializer<List<Game>> CreateSerializer() => SelectedFormat switch
    {
        "TXT" => new TXTSerializer(),
        "CSV" => new CSVSerializer(),
        "BIN" => new BINSerializer(),
        _ => new JSONSerializer()
    };

    private void ClearMessages() { ErrorMessage = null; SuccessMessage = null; }
}
