using Unity.Cinemachine;
using UnityEngine;

namespace CombatTower.Game.Gameplay
{
    public class LockOnCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera m_camera;
        [SerializeField] private float m_closestRangeToLookTarget = 2f;
        [SerializeField] private float m_lookTargetPositionChangeSpeed = 5f;
        [SerializeField] private float m_lookTargetMaxYPosition; // TEMP - must get from target
        [SerializeField] private float m_lookTargetMinYPosition; // TEMP - must get from target
        
        private Transform _trackingTarget;
        private Transform _lookTarget;

        public void SetLookTarget(Transform lookTarget)
        {
            if (_lookTarget != null)
            {
                var pos = _lookTarget.localPosition;
                pos.y = m_lookTargetMinYPosition;
                _lookTarget.localPosition = pos;
            }

            _lookTarget = lookTarget;
            m_camera.Target.LookAtTarget = _lookTarget;
        }

        private void Start()
        {
            _trackingTarget = m_camera.Target.TrackingTarget;
        }
        
        private void Update()
        {
            if (_lookTarget == null) return;

            var targetLocalPos = _lookTarget.localPosition;

            if (Vector3.Distance(_trackingTarget.position, _lookTarget.position) <= m_closestRangeToLookTarget)
            {
                targetLocalPos.y = m_lookTargetMaxYPosition;
            }
            else
            {
                targetLocalPos.y = m_lookTargetMinYPosition;
            }

            _lookTarget.localPosition = Vector3.Lerp(_lookTarget.localPosition, targetLocalPos, m_lookTargetPositionChangeSpeed * Time.deltaTime);
        }
    }
}
