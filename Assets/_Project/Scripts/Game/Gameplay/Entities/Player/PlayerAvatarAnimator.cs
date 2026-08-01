using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class PlayerAvatarAnimator : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_characterRigidbody;
        [SerializeField] private Animator m_animator;

        private const string _forwardMoveFloat = "ForwardMove";
        private const string _strifeMoveFloat = "SidewardMove";
        private const float _movementThreshold = 0.05f;

        private const string _battleStateBool = "BattleState";
        private const string _attackTrigger = "Attack";

        private PlayerAvatarMovement _movement;
        private bool _isActive = true;

        public void Bind(PlayerAvatarMovement movement)
        {
            _movement = movement;
        }

        public void SetMovementAnimationActive(bool state) => _isActive = state;

        public void PlayAttack()
        {
            m_animator.SetTrigger(_attackTrigger);
            m_animator.SetBool(_battleStateBool, true);
        }

        public void SetBattleState(bool state) => m_animator.SetBool(_battleStateBool, state);

        private void Update()
        {
            if (_movement != null)
            {
                var localLookDirection = _movement.GetLocalLookDirection();

                m_animator.SetFloat(_forwardMoveFloat, _isActive ? localLookDirection.z : 0f);
                m_animator.SetFloat(_strifeMoveFloat, _isActive ? localLookDirection.x : 0f);
            }
            else
            {
                m_animator.SetFloat(_forwardMoveFloat, m_characterRigidbody.linearVelocity.magnitude >= _movementThreshold && _isActive ? 1f : 0f);
                m_animator.SetFloat(_strifeMoveFloat, 0f);
            }
        }

        /*public void PlaySimpleAttack()
        {
            m_animator.SetTrigger(_simpleAttackTrigger);
        }

        public void PlaySimpleAttack2()
        {
            m_animator.SetTrigger(_simpleAttackTrigger2);
        }

        public void PlaySimpleAttack3()
        {
            m_animator.SetTrigger(_simpleAttackTrigger3);
        }

        public void PlaySuperAttack()
        {
            m_animator.SetTrigger(_superAttackTrigger);
        }

        public void PlayDeath()
        {
            m_animator.SetTrigger(_deathTrigger);
        }

        public void PlayWin()
        {
            m_animator.SetTrigger(_winTrigger);
        }

        public void SetAsCalm()
        {
            m_animator.SetTrigger(_calmTrigger);
        }

        public void PlayEquip(System.Action onEnd)
        {
            m_animator.SetTrigger(_equipTrigger);

            StartCoroutine(EndEquipAnimationRoutine(onEnd));
        }

        private IEnumerator EndEquipAnimationRoutine(System.Action onEnd)
        {
            yield return new WaitForSeconds(1.5f);

            onEnd?.Invoke();
        }*/
    }
}