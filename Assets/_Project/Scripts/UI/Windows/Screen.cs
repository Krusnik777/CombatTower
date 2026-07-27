namespace UI.Windows
{
    public abstract class Screen : Window
    {
        public Screen(IWindowView view) : base(view) { }
    }
}
