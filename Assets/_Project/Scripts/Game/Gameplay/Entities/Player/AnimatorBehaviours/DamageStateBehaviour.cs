using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class DamageStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            var eventsCollector = animator.GetComponent<AnimatorEventsCollector>();

            if (eventsCollector == null) return;

            eventsCollector.OnHitRecovery();
        }
    }
}
