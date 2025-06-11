using Sample.SP0.Client.Core.View;
using System.Windows;
using System.Windows.Controls;

namespace Sample.SP0.Client.View
{
    /// <summary>
    /// StartPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StartPanel : UserControl, IStartPanel
    {
        public event EventHandler StartButtonClickedEvent;
        public event EventHandler StartOfflineButtonClickedEvent;

        public StartPanel()
        {
            InitializeComponent();

            StartButtonClickedEvent = (sender, args) => { };
            StartOfflineButtonClickedEvent = (sender, args) => { };
        }

        public string GetAppKey()
        {
            return AppKeyInput.Password ?? string.Empty;
        }

        public string GetSecretKey()
        {
            return SecretKeyInput.Password ?? string.Empty;
        }

        private void OnStartButtonClicked(object sender, RoutedEventArgs e)
        {
            StartButtonClickedEvent?.Invoke(this, e);
        }

        private void OnStartOfflineButtonClicked(object sender, RoutedEventArgs e)
        {
            StartOfflineButtonClickedEvent?.Invoke(this, e);
        }
    }
}
