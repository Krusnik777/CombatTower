using UnityEngine;

namespace Utils
{
    public class LookAtCamera : MonoBehaviour
    {
        private enum Mode
        {
            LookAt,
            LookAtInverted,
            CameraForward,
            CameraForwardInverted,
        }

        [SerializeField] private Mode m_mode = Mode.LookAt;
        [SerializeField] private Camera m_customCamera;

        private void LateUpdate()
        {
            var targetCamera = m_customCamera != null ? m_customCamera : Camera.main;

            if (targetCamera == null)
                return;

            switch (m_mode)
            {
                case Mode.LookAt:
                    transform.LookAt(targetCamera.transform);
                    break;
                case Mode.LookAtInverted:
                    Vector3 dirFromCamera = transform.position - targetCamera.transform.position;
                    transform.LookAt(transform.position + dirFromCamera);
                    break;
                case Mode.CameraForward:
                    transform.forward = targetCamera.transform.forward;
                    break;
                case Mode.CameraForwardInverted:
                    transform.forward = -targetCamera.transform.forward;
                    break;
            }
        }
    }
}