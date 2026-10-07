using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class PatrolPointView : MonoBehaviour
    {
        [field: SerializeField] public float WaitingTime { get; private set; }
        [field: SerializeField] public string AnimationName { get; private set; }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
        #endif
    }
}
