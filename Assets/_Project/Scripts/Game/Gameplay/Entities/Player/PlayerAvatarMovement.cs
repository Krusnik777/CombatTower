using CombatTower.Game.Services;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class PlayerAvatarMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_characterRigidbody;

        public bool IsControlledByRootMotion { get; set; }

        private const float _keyboardInputUpdateSpeed = 10f;

        private GameInputService _gameInputService;
        private float _movementSpeed;
        private float _rotationSpeed;

        private Vector3 _directionControl;
        private Vector3 _rotationDirectionControl;
        private Transform _lookTargetTransform;
        private bool _forceRotationDirection;

        private bool _isActive = true;

        public Vector3 GetLocalLookDirection(Vector3 customDirection = new Vector3()) => m_characterRigidbody.transform.InverseTransformDirection(customDirection == Vector3.zero ? _directionControl : customDirection);
        public float GetDeltaAngleBetweenDirectionAndLookTarget(Vector3 direction)
        {
            if (_lookTargetTransform == null) return 0f;

            var directionToTarget = (_lookTargetTransform.position - transform.position).normalized;
            directionToTarget.y = 0;

            return Vector3.SignedAngle(directionToTarget, direction, Vector3.up);
        }

        public void Bind(GameInputService gameInputService, float movementSpeed = 1f, float rotationSpeed = 1f)
        {
            _gameInputService = gameInputService;
            _movementSpeed = movementSpeed;
            _rotationSpeed = rotationSpeed;
        }

        public void SetRotationDirection(Vector3 direction, float rotationSpeed = 1f, bool forceDirection = false)
        {
            _rotationDirectionControl = direction;
            _rotationSpeed = rotationSpeed;
            _forceRotationDirection = forceDirection;
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

                if (_lookTargetTransform == null || _forceRotationDirection)
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
            var isGamepad = InputDeviceDetectService.CurrentControlDevie is UnityEngine.InputSystem.Gamepad;
            var keyboardDirection = moveDirection != Vector3.zero ? Vector3.Lerp(_directionControl, moveDirection, _keyboardInputUpdateSpeed * Time.deltaTime) : Vector3.zero;
            _directionControl = isGamepad ? moveDirection : keyboardDirection;
            //_directionControl.Normalize();
        }
    }
}
