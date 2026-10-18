using UnityEngine;

namespace CombatTower
{
    public class TransformFollower : MonoBehaviour
    {
        [SerializeField] private Transform m_followTarget;

        void Update()
        {
            transform.position = m_followTarget.transform.position;
        }
    }
}
