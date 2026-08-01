using CombatTower.Game.Services;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class PlayerAvatarMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_characterRigidbody;
        [SerializeField] private float m_movementSpeed = 5f;
        [SerializeField] private float m_rotateSpeed = 500f;

        public Vector3 DirectionControl => _directionControl;
        public bool IsControlledByRootMotion { get; set; }

        private GameInputService _gameInputService;

        private Vector3 _directionControl;
        private Transform _lookTargetTransform;

        private bool _isActive = true;

        public Vector3 GetLocalLookDirection() => m_characterRigidbody.transform.InverseTransformDirection(_directionControl);

        public void Bind(GameInputService gameInputService)
        {
            _gameInputService = gameInputService;
        }
        public void SetActive(bool state) => _isActive = state;
        public void Stop()
        {
            m_characterRigidbody.linearVelocity = Vector3.zero;
            m_characterRigidbody.angularVelocity = Vector3.zero;
        }
        public void SetLookTransform(Transform target) => _lookTargetTransform = target;

        public void Teleport(Transform targetPlace)
        {
            m_characterRigidbody.position = targetPlace.position;
            m_characterRigidbody.rotation = targetPlace.rotation;
        }

        private void FixedUpdate()
        {
            GetMoveDirection();

            if (_directionControl.magnitude > 0 && _isActive)
            {
                Quaternion targetRotation;

                if (_lookTargetTransform == null)
                {
                    targetRotation = Quaternion.LookRotation(_directionControl);
                }
                else
                {
                    var directionToTarget = (_lookTargetTransform.position - transform.position).normalized;
                    targetRotation = Quaternion.LookRotation(directionToTarget);
                }

                m_characterRigidbody.linearVelocity = _directionControl * m_movementSpeed * Time.fixedDeltaTime;
                m_characterRigidbody.rotation = Quaternion.Lerp(m_characterRigidbody.rotation, targetRotation, m_rotateSpeed * Time.fixedDeltaTime);
            }
            else
            {
                m_characterRigidbody.linearVelocity = Vector3.zero;
            }
        }

        private void OnAnimatorMove()
        {
            if (!IsControlledByRootMotion) return;

            var animator = GetComponent<Animator>();
            Vector3 rootDelta = animator.deltaPosition;
            Quaternion rootDeltaRotation = animator.deltaRotation;

            m_characterRigidbody.MovePosition(m_characterRigidbody.position + rootDelta);
            m_characterRigidbody.MoveRotation(m_characterRigidbody.rotation * rootDeltaRotation);
        }

        private void GetMoveDirection()
        {
            if (_gameInputService == null) return;

            var moveDirection = _gameInputService.GetMovementInput();
            _directionControl = moveDirection;
            //_directionControl.Normalize();
        }
    }
}
