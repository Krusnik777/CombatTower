using UnityEngine;

namespace CombatTower.Game.Configs
{
    [CreateAssetMenu(fileName = "InteractiveLayerMasksConfig", menuName = "Scriptable Objects/Interactive Layer Masks Config")]
    public class InteractiveLayerMasksConfig : ScriptableObject
    {
        [field: SerializeField] public LayerMask Player { get; private set; }
        [field: SerializeField] public LayerMask Enemy { get; private set; }
    }
}
