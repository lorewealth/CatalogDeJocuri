using System.ComponentModel;

namespace WPF.Tools;

public class ComboBoxFilter<T>(T value) : INotifyPropertyChanged
{
    private bool _isSelected;

    public T Value { get; } = value;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;
            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public string DisplayName => Value?.ToString()?.Replace("_", " ") ?? "Unknown";
    public event PropertyChangedEventHandler? PropertyChanged;
}
