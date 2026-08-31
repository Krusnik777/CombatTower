using UnityEngine;

namespace CombatTower.Game.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/Game Settings/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [field: Header("Battle Grid")]
        [field: SerializeField] public int GridCapacity { get; private set; } = 12;
        [field: SerializeField] public int AttackCapacity { get; private set; } = 10;
    }
}
