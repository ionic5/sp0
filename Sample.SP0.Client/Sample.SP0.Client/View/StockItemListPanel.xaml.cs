using Sample.SP0.Client.Core.Entities;
using Sample.SP0.Client.Core.View;
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
    /// StockItemListPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StockItemListPanel : UserControl
    {
        public event EventHandler<ListItemChangedEventArgs> StockItemSelectedEvent;

        public StockItemListPanel()
        {
            InitializeComponent();

            StockItemSelectedEvent = (sender, args) => { };
        }

        public void OnStockItemSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var listView = sender as ListView;
            if (listView != null && listView.SelectedIndex != -1)
            {
                int selectedIndex = listView.SelectedIndex;
                StockItemSelectedEvent(this, new ListItemChangedEventArgs(listView.SelectedIndex));
            }
        }

        public void SetStockItmes(IEnumerable<StockItemInfo> items)
        {
            foreach (var item in items)
            {
                var listitem = new ListViewItem();
                listitem.Content = item.CompanyName;

                StockItemList.Items.Add(listitem);
            }
        }

        public void SelectFirstItem()
        {
            if (StockItemList.Items.Count > 0)
                StockItemList.SelectedIndex = 0;
        }
    }
}
