using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class PlayerView : MonoBehaviour
    {
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public PlayerAvatarMovement Movement { get; private set; }
        [field: SerializeField] public AnimatorEventsCollector EventsCollector { get; private set; }
        [field: SerializeField] public Transform BeltWeaponTransform { get; private set; }
        [field: SerializeField] public Transform WeaponHolderTransform { get; private set; }
    }
}
