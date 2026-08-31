using UnityEngine;

namespace CombatTower.Game.Gameplay.Battle
{
    public class GridSlot
    {
        public Transform ClosePoint { get; }
        public Transform FarPoint { get; }
        //public Enemy BusyBy { get; }
        
        public GridSlot(Transform closePosition, Transform farPosition)
        {
            ClosePoint = closePosition;
            FarPoint = farPosition;
        }
    }
}
