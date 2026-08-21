using System;
using CombatTower.Game.Gameplay.HealthSystem;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class Enemy : IDisposable
    {
        public Subject<Enemy> OnDeath { get; }
        public Subject<Damage> OnDamage { get; }

        public Health Health { get; }

        private EnemyView _view;

        private IDisposable _damageableListenerDisposable;
        private IDisposable _deathDisposable;

        public Enemy(EnemyView view)
        {
            _view = view;

            Health = new(new DamageProcessor(), 10); // healthValue is TEMP

            _damageableListenerDisposable = _view.Damageable.OnHitted.Subscribe(TakeDamage);
        }

        public void Dispose()
        {
            _damageableListenerDisposable?.Dispose();
            _deathDisposable?.Dispose();
        }

        private void TakeDamage(Damage damage)
        {
            /*bool isBlocked = UnityEngine.Random.value >= 1 - someArmorDefenceChance;
            if (isBlocked) damage.Modifiers.Add(new ArmorDefenceModifier(someArmorValue));*/

            if (Health.TakeDamage(ref damage, out int resultedHealthValue))
            {
                UnityEngine.Debug.Log("ENEMY HIT");

                if (resultedHealthValue <= 0)
                {
                    UnityEngine.Debug.Log("ENEMY DEAD");

                    _view.enabled = false;

                    _deathDisposable = Observable.Interval(TimeSpan.FromSeconds(3f)).Subscribe(_ =>
                    {
                        _view.gameObject.SetActive(false);

                        Dispose();
                    });

                    // Stop all inner processes
                    OnDeath?.OnNext(this);
                }
                else
                {
                    // Show hit effect ?
                    OnDamage?.OnNext(damage);
                    //_sounds.Play(PlayerSounds.Damage); // ???
                }
            }
        }
    }
}
