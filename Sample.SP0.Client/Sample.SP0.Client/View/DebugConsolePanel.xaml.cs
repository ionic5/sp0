using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sample.SP0.Client.View
{
    /// <summary>
    /// DebugConsolePanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DebugConsolePanel : UserControl
    {
        public DebugConsolePanel()
        {
            InitializeComponent();
        }

        public void AddDebugMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                var newTextBox = new TextBox
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 20),
                    IsReadOnly = true, // 수정 불가능하게 설정
                    BorderThickness = new Thickness(0), // 테두리 제거
                    Background = Brushes.Transparent // 배경을 투명하게 설정
                };
                ContentPanel.Children.Add(newTextBox);

                while (ContentPanel.Children.Count > 100)
                    ContentPanel.Children.RemoveAt(0);

                ScrollViewer.UpdateLayout();
                ScrollViewer.ScrollToEnd();
            });
        }
    }
}
