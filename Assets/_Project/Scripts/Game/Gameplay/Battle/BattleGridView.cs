using UnityEngine;

namespace CombatTower.Game.Gameplay.Battle
{
    public class BattleGridView : MonoBehaviour
    {
        [field: SerializeField] public int SlotAmount { get; private set; } = 8;
        [field: SerializeField] public float CloseRadius { get; private set; } = 2.5f;
        [field: SerializeField] public float FarRadius { get; private set; } = 5f;
    }
}
