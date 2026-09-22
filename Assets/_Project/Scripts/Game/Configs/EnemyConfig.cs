using UnityEngine;

namespace CombatTower.Game.Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string PrefabName { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int MaxHealth { get; private set; } = 10;
    }
}
