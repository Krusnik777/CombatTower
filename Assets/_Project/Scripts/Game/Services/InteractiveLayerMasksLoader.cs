using CombatTower.Game.Configs;
using UnityEngine;

namespace CombatTower.Game.Services
{
    public class InteractiveLayerMasksLoader
    {
        public void LoadMasksAndAssignToLayerMasks()
        {
            var config = Resources.Load<InteractiveLayerMasksConfig>("Settings/InteractiveLayerMasksConfig");

            Root.LayerMasks.Player = config.Player;
            Root.LayerMasks.Enemy = config.Enemy;
        }
    
    }
}
