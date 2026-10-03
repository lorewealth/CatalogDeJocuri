using System.Windows;
using APISearch;
using Microsoft.Extensions.DependencyInjection;
using Stocking.Database;
using WPF.Stores;
using WPF.ViewModels;
using WPF.Views;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _services = null!;
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ServiceCollection services = [];

            services.AddSingleton<GameStore>();
            services.AddSingleton<GamesViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton(_ => new SQLStocking("GameCatalog.db"));
            services.AddSingleton<ManagingCache>();

            services.AddTransient<APIAddingViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<ImportExportViewModel>();
            services.AddTransient<ManualAddingViewModel>();
            services.AddTransient<ModdifyViewModel>();
            services.AddTransient<PriceViewModel>();
            services.AddTransient<SearchViewModel>();

            _services = services.BuildServiceProvider();
            await _services.GetRequiredService<GamesViewModel>().GetGamesAsync();

            _services.GetRequiredService<MainWindow>().Show();
        }
    }

}
