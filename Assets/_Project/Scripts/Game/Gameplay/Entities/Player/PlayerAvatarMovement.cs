using CombatTower.Game.Services;
using Unity.VisualScripting;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class PlayerAvatarMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_characterRigidbody;

        public bool IsControlledByRootMotion { get; set; }

        private GameInputService _gameInputService;
        private float _movementSpeed;
        private float _rotationSpeed;

        private Vector3 _directionControl;
        private Vector3 _rotationDirectionControl;
        private Transform _lookTargetTransform;

        private bool _isActive = true;

        public Vector3 GetLocalLookDirection() => m_characterRigidbody.transform.InverseTransformDirection(_directionControl);

        public void Bind(GameInputService gameInputService, float movementSpeed = 1f, float rotationSpeed = 1f)
        {
            _gameInputService = gameInputService;
            _movementSpeed = movementSpeed;
            _rotationSpeed = rotationSpeed;
        }

        public void SetRotationDirection(Vector3 direction, float rotationSpeed = 1f)
        {
            _rotationDirectionControl = direction;
            _rotationSpeed = rotationSpeed;
        }

        public void SetActive(bool state) => _isActive = state;
        public void SetLookTransform(Transform target) => _lookTargetTransform = target;

        public void Stop()
        {
            m_characterRigidbody.linearVelocity = Vector3.zero;
            m_characterRigidbody.angularVelocity = Vector3.zero;
        }

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
                    directionToTarget.y = 0;
                    targetRotation = Quaternion.LookRotation(directionToTarget);
                }

                m_characterRigidbody.linearVelocity = _directionControl * _movementSpeed * Time.fixedDeltaTime;
                m_characterRigidbody.rotation = Quaternion.Lerp(m_characterRigidbody.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
            }
            else
            {
                m_characterRigidbody.linearVelocity = Vector3.zero;

                if (_lookTargetTransform == null)
                {
                    if (_rotationDirectionControl != Vector3.zero)
                    {
                        var targetRotation = Quaternion.LookRotation(_rotationDirectionControl);
                        m_characterRigidbody.rotation = Quaternion.Lerp(m_characterRigidbody.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);

                        var angle = Quaternion.Angle(m_characterRigidbody.rotation, targetRotation);
                        if (angle <= 5)
                        {
                            m_characterRigidbody.rotation = targetRotation;
                            _rotationDirectionControl = Vector3.zero;
                        }
                    }
                }
                else
                {
                    var directionToTarget = (_lookTargetTransform.position - transform.position).normalized;
                    directionToTarget.y = 0;
                    var targetRotation = Quaternion.LookRotation(directionToTarget);
                    m_characterRigidbody.rotation = Quaternion.Lerp(m_characterRigidbody.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);

                    if (_rotationDirectionControl != Vector3.zero) _rotationDirectionControl = Vector3.zero;
                }
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
            if (_gameInputService == null)
            {
                _directionControl = Vector3.zero;
                return;
            }

            var moveDirection = _gameInputService.GetMovementInput();
            _directionControl = moveDirection;
            //_directionControl.Normalize();
        }
    }
}
