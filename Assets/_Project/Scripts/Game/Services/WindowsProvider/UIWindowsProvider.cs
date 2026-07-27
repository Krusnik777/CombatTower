using System;
using System.Collections.Generic;
using UI.Windows;

namespace CombatTower.Game.Services
{
    public class UIWindowsProvider : IDisposable
    {
        private IWindowsFactory _windowsFactory;

        private Dictionary<Type, Screen> _createdScreensMap;
        private Screen _activeScreen;

        public UIWindowsProvider(IWindowsFactory windowsFactory)
        {
            _windowsFactory = windowsFactory;

            _createdScreensMap = new();
        }

        public T GetScreen<T>() where T : Screen
        {
            if (_activeScreen is T) return _activeScreen as T;

            _activeScreen?.Hide();

            if (!_createdScreensMap.ContainsKey(typeof(T)))
            {
                var newScreen = _windowsFactory.CreateScreen<T>();
                _createdScreensMap.Add(typeof(T), newScreen);
            }

            var screen = _createdScreensMap[typeof(T)] as T;
            _activeScreen = screen;

            return screen;
        }

        public T ShowScreen<T>() where T : Screen
        {
            var screen = GetScreen<T>();

            screen.Show();

            return screen;
        }

        public void Dispose()
        {
            foreach (var screen in _createdScreensMap)
            {
                screen.Value?.Dispose();
            }
        }
    }
}
