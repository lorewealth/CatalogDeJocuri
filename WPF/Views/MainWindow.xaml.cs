using AboutGame;
using AboutGame.Enums;
using APISearch;
using Microsoft.Win32;
using Stocking.Packaging;
using Stocking.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPF.Validators;
using WPF.ViewModels;

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
