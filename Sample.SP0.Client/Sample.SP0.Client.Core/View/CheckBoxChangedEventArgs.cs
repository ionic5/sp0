namespace Sample.SP0.Client.Core.View
{
    public class CheckBoxChangedEventArgs : EventArgs
    {
        public readonly bool IsEnabled;

        public CheckBoxChangedEventArgs(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }
}
