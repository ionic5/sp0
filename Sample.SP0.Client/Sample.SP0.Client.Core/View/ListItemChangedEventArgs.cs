namespace Sample.SP0.Client.Core.View
{
    public class ListItemChangedEventArgs
    {
        public readonly int SelectedIndex;

        public ListItemChangedEventArgs(int selectedIndex)
        {
            SelectedIndex = selectedIndex;
        }
    }
}
