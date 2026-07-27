using System;
using UnityEngine;
using Screen = UI.Windows.Screen;
using WindowView = UI.Windows.WindowView;

namespace CombatTower.Game.Services
{
    public class GameplayWindowsFactory : IWindowsFactory
    {
        private const string _beforeBattleScreenViewName = "BeforeBattleScreenView";
        private const string _battleScreenViewName = "BattleScreenView";
        private const string _victoryScreenViewName = "VictoryScreenView";
        private const string _defeatScreenViewName = "DefeatScreenView";

        private Transform _screensHolder;
        private Transform _popupsHolder;

        public GameplayWindowsFactory(Transform screensHolder, Transform popupsHolder)
        {
            _screensHolder = screensHolder;
            _popupsHolder = popupsHolder;
        }

        public virtual T CreateScreen<T>() where T : Screen
        {
            Type t = typeof(T);

            /*if (t == typeof(BeforeBattleScreen))
            {
                var prefabPath = GetPrefabPath(_beforeBattleScreenViewName);
                var view = InstantiateWindowViewForScreen<BeforeBattleScreenView>(prefabPath);

                return new BeforeBattleScreen(view) as T;
            }

            if (t == typeof(BattleScreen))
            {
                var prefabPath = GetPrefabPath(_battleScreenViewName);
                var view = InstantiateWindowViewForScreen<BattleScreenView>(prefabPath);

                return new BattleScreen(view) as T;
            }

            if (t == typeof(VictoryScreen))
            {
                var prefabPath = GetPrefabPath(_victoryScreenViewName);
                var view = InstantiateWindowViewForScreen<VictoryScreenView>(prefabPath);

                return new VictoryScreen(view) as T;
            }

            if (t == typeof(DefeatScreen))
            {
                var prefabPath = GetPrefabPath(_defeatScreenViewName);
                var view = InstantiateWindowViewForScreen<DefeatScreenView>(prefabPath);

                return new DefeatScreen(view) as T;
            }*/

            throw new ArgumentNullException($"Unsupported class - type of: {t}");
        }

        private T InstantiateWindowViewForScreen<T>(string prefabPath) where T : WindowView
        {
            var prefab = Resources.Load<T>(prefabPath);
            var windowView = GameObject.Instantiate(prefab, _screensHolder);

            return windowView;
        }

        private string GetPrefabPath(string viewName)
        {
            return $"Prefabs/UI/Gameplay/Screens/{viewName}";
        }
    }
}