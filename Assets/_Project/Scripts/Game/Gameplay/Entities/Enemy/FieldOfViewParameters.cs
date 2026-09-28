using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class FieldOfViewParameters : MonoBehaviour
    {
        [field: SerializeField, Min(0f)] public float Radius { get; private set; } = 10f;
        [field: SerializeField, Range(0f, 360f)] public float Angle { get; private set; } = 90f;
        [field: SerializeField] public Vector3 ViewOffset { get; private set; } = new Vector3(0f, 1.5f, 0f);
        [Header("Gizmos")]
        [SerializeField] private Color m_gizmoColor = new Color(1f, 0.6f, 0.1f, 1f);
        [SerializeField, Min(8)] private int m_gizmoArcSteps = 32;

        public Vector3 EyePosition => transform.TransformPoint(ViewOffset);

        private void OnDrawGizmos()
        {
            Vector3 eye = EyePosition;
            Vector3 forward = transform.forward;
            float half = Angle * 0.5f;

            Gizmos.color = m_gizmoColor;
            Gizmos.DrawWireSphere(eye, Radius);

            Vector3 leftDir = Quaternion.AngleAxis(-half, Vector3.up) * forward;
            Vector3 rightDir = Quaternion.AngleAxis(half, Vector3.up) * forward;

            Gizmos.DrawLine(eye, eye + leftDir * Radius);
            Gizmos.DrawLine(eye, eye + rightDir * Radius);

            int steps = Mathf.Max(8, m_gizmoArcSteps);
            float stepAngle = Angle / steps;

            Vector3 prev = eye + leftDir * Radius;
            for (int i = 1; i <= steps; i++)
            {
                float a = -half + stepAngle * i;
                Vector3 dir = Quaternion.AngleAxis(a, Vector3.up) * forward;
                Vector3 next = eye + dir * Radius;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
}
