using UnityEngine;
using Random = UnityEngine.Random;

namespace CombatTower.Game.Gameplay.HealthSystem
{
    public static class DamageFactory
    {
        public static Damage Create(Transform attackerTransform, Transform attackedTransform/*AttackData data*/)
        {
            //bool isCritical = Random.value >= 1 - data.CriticalChance;
            //bool isArmorBreak = Random.value >= 1 - data.ArmorBreakChance;

            var damageValue = Random.Range(1, 5); // Temp

            var damage = new Damage()
            {
                BaseValue = /*data.Damage*/damageValue,
                ResultValue = /*data.Damage*/damageValue,
                Modifiers = new()
            };

            if (IsAttackedBehind(attackerTransform, attackedTransform)) damage.Direction = DamageDirection.Back;

            //if (isCritical) damage.Modifiers.Add(new CriticalDamageModifier());
            //if (isArmorBreak) damage.Modifiers.Add(new ArmorBreakDamageModifier());

            return damage;
        }

        private static bool IsAttackedBehind(Transform attackerTransform, Transform attackedTransform, float behindAngle = 60f)
        {
            Vector3 local = attackedTransform.InverseTransformPoint(attackerTransform.position);
            local.y = 0f;

            float angle = Vector3.Angle(Vector3.forward, local.normalized);
            
            return angle > (180f - behindAngle);
        }
    }
}
