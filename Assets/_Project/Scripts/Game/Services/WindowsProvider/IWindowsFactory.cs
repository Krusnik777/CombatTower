using Screen = UI.Windows.Screen;
//using Popup = UI.Windows.Popup;

namespace CombatTower.Game.Services
{
    public interface IWindowsFactory
    {
        public T CreateScreen<T>() where T : Screen;
    }
}
