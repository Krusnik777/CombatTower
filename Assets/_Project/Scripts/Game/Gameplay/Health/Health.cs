using R3;

namespace CombatTower.Game.Gameplay.HealthSystem
{
    public class Health
    {
        public int MaxValue { get; }
        public Observable<int> Value => _currentValue;

        public string HealthStatus => $"{_currentValue.Value}/{MaxValue}";

        private IDamageProcessor _damageProcessor;

        private ReactiveProperty<int> _currentValue;

        private bool _ignoreDamage;

        public Health(IDamageProcessor damageProcessor, int maxValue)
        {
            MaxValue = maxValue;
            _damageProcessor = damageProcessor;
            
            _currentValue = new(MaxValue);
        }

        public void SetIgnoreDamage(bool state) => _ignoreDamage = state;

        public bool TakeDamage(ref Damage damage, out int resultedHealthValue)
        {
            resultedHealthValue = _currentValue.Value;
            if (_currentValue.Value <= 0) return false;

            if (_ignoreDamage) return false;

            _damageProcessor.Process(ref damage);

            resultedHealthValue -= damage.ResultValue;

            if (resultedHealthValue <= 0)
            {
                _currentValue.Value = 0;
            }
            else
            {
                _currentValue.Value = resultedHealthValue;
            }

            return true;
        }

        public bool TryHeal(int healAmount, out int healedAmount)
        {
            healedAmount = 0;
            if (_currentValue.Value >= MaxValue) return false;

            var value = _currentValue.Value;
            value += healAmount;

            if (value >= MaxValue)
            {
                healedAmount = MaxValue - _currentValue.Value;

                _currentValue.Value = MaxValue;
            }
            else
            {
                healedAmount = healAmount;

                _currentValue.Value = value;
            }

            return true;
        }
    }
}
