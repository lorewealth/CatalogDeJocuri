using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StocareJocurilor;
using CatalogDeJocuri;
using DespreJoc;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IStocare AdministrJocuri = Decident.PrelucrareaDatelor();
        List<Joc> Jocuri;

        public MainWindow()
        {
            InitializeComponent();
            Jocuri = AdministrJocuri.GetJocuri();
            MessageBox.Show(Jocuri.Count().ToString());

            DescJocSing.Text += string.Join("\n========================================\n", Jocuri.Select(joc => joc.GetInfo())) ;
            
        }
    }
}