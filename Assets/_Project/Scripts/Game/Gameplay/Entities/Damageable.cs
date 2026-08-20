using CombatTower.Game.Gameplay.HealthSystem;
using R3;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities
{
    public class Damageable : MonoBehaviour, IDamageable
    {
        public Subject<Damage> OnHitted { get; private set; } = new();

        public void Hit(Damage damage)
        {
            OnHitted?.OnNext(damage);
        }
    }
}
