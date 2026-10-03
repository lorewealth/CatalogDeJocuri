using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using WPF.Tools;

namespace WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IServiceProvider _services;
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }
        public ICommand HomeNavCommand { get; }
        public ICommand ManualAddNavCommand { get; }
        public ICommand ManualSearchNavCommand { get; }
        public ICommand ModifyGameNavCommand { get; }
        public ICommand OnlineSearchNavCommand { get; }
        public ICommand PriceGraphNavCommand { get; }
        public ICommand ImportExportNavCommand { get; }
        
        public MainViewModel(IServiceProvider services)
        {
            _services = services;
            _currentViewModel = GetViewModel<HomeViewModel>();

            HomeNavCommand          = new RelayCommand(_ => Navigate<HomeViewModel>());
            ManualAddNavCommand     = new RelayCommand(_ => Navigate<ManualAddingViewModel>());
            ManualSearchNavCommand  = new RelayCommand(_ => Navigate<SearchViewModel>());
            ModifyGameNavCommand    = new RelayCommand(_ => Navigate<ModdifyViewModel>());
            OnlineSearchNavCommand  = new RelayCommand(_ => Navigate<APIAddingViewModel>());
            PriceGraphNavCommand    = new RelayCommand(_ => Navigate<PriceViewModel>());
            ImportExportNavCommand  = new RelayCommand(_ => Navigate<ImportExportViewModel>());
        }

        private T GetViewModel<T>() where T : BaseViewModel
        {
            return _services.GetRequiredService<T>();
        }
        private void Navigate<T>() where T : BaseViewModel
        {
            CurrentViewModel = GetViewModel<T>();
        }
    }
}
