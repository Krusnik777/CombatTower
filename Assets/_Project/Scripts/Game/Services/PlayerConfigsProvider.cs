using CombatTower.Game.Configs;
using UnityEngine;

namespace CombatTower.Game.Services
{
    public class PlayerConfigsProvider
    {
        public PlayerParametersConfig ParametersConfig { get; }

        public PlayerConfigsProvider()
        {
            ParametersConfig = Resources.Load<PlayerParametersConfig>("Settings/PlayerParametersConfig");
        }
    
    }
}
