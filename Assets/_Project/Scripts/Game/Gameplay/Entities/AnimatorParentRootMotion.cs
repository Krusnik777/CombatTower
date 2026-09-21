using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorParentRootMotion : MonoBehaviour
    {
        private Animator animator;

        private bool _isActive = true;

        public bool SetRootMotionActive(bool state) => _isActive = state;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorMove()
        {
            if (!_isActive) return;

            if (transform.parent != null)
            {
                transform.parent.position += animator.deltaPosition;
                transform.parent.rotation *= animator.deltaRotation;
            }
            else
            {
                transform.position += animator.deltaPosition;
                transform.rotation *= animator.deltaRotation;
            }
        }
    }
}