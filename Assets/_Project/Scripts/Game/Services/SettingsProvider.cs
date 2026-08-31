using CombatTower.Game.Settings;
using UnityEngine;

namespace CombatTower.Game.Services
{
    public class SettingsProvider : ISettingsProvider
    {
        public ApplicationSettings ApplicationSettings { get; }

        public GameSettings GameSettings { get; }

        public SettingsProvider()
        {
            ApplicationSettings = Resources.Load<ApplicationSettings>("Settings/ApplicationSettings");
            GameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
        }
    }
}
