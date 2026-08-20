using System;
using System.Collections.Generic;

namespace CombatTower.Game.Gameplay.HealthSystem
{
    public class DamageProcessor : IDamageProcessor
    {
        private Dictionary<Type, IDamageModifier> _passiveModifiersMap;

        public DamageProcessor()
        {
            _passiveModifiersMap = new();
        }
        
        public void Process(ref Damage damage)
        {
            foreach (var modifier in damage.Modifiers) modifier.Modify(ref damage);
            foreach (var passive in _passiveModifiersMap.Values) passive.Modify(ref damage);

            if (damage.ResultValue <= 0)
            {
                damage.IsCritical = false;
                damage.ResultValue = 1;
            }
        }

        public void AddPassiveModifier<T>(T damageModifier, bool replaceIfContains = false) where T : IDamageModifier
        {
            var modifierType = typeof(T);

            if (_passiveModifiersMap.ContainsKey(modifierType) && !replaceIfContains) return;

            _passiveModifiersMap[modifierType] = damageModifier;
        }

        public void RemovePassiveModifier<T>() where T : IDamageModifier
        {
            var modifierType = typeof(T);

            if (_passiveModifiersMap.ContainsKey(modifierType)) _passiveModifiersMap.Remove(modifierType);
        }
    }
}
