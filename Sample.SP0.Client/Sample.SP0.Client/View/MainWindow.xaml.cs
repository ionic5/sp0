using Sample.SP0.Client.Core;
using Sample.SP0.Client.Core.KiwoomApi;
using Sample.SP0.Client.Core.WebApi;
using Sample.SP0.Client.View;
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
using System.Windows.Threading;

namespace Sample.SP0.Client.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var logger = new Logger(DebugConsolePanel);

            var urlSet = new UrlSet("https://api.kiwoom.com");

            var restApiClient = new RestApiClient(logger);
            var tokenStore = new TokenStore();

            var ctrl = new StartPanelController(StartPanel, restApiClient, logger, tokenStore, urlSet);
            StartPanel.StartButtonClickedEvent += ctrl.OnStartButtonClickedEvent;
        }
    }
}