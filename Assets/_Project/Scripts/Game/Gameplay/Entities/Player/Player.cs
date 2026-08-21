using CombatTower.Game.Configs;
using CombatTower.Game.Gameplay.HealthSystem;
using DI;
using R3;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class Player : System.IDisposable
    {
        public Subject<Player> OnDeath { get; }
        public Subject<Damage> OnDamage { get; }

        public PlayerParametersConfig ParametersConfig { get; }
        public Health Health { get; }
        public bool IsNotStaggeredByDamage { get; set; }

        private PlayerView _view;
        public Rigidbody Rigidbody => _view.Rigidbody;
        public Animator Animator => _view.Animator;
        public PlayerAvatarMovement Movement => _view.Movement;
        public AnimatorEventsCollector EventsCollector => _view.EventsCollector;
        public Transform WeaponHolderTransform => _view.WeaponHolderTransform; // TEMP ?

        private PlayerWeaponStateMachine _playerWeaponStateMachine;
        private CompositeDisposable _changesListenerDisposables;

        public Player(PlayerParametersConfig config, PlayerView view, DIContainer sceneContainer)
        {
            ParametersConfig = config;
            _view = view;

            Health = new(new DamageProcessor(), 100); // healthValue is TEMP
            OnDamage = new();
            OnDeath = new();

            _playerWeaponStateMachine = new(this, sceneContainer);

            _changesListenerDisposables = new()
            {
                _view.Damageable.OnHitted.Subscribe(TakeDamage),
                _view.EventsCollector.OnFootstep.Subscribe(OnStep)
            };
        }

        public void Dispose()
        {
            _playerWeaponStateMachine?.Dispose();
            _changesListenerDisposables?.Dispose();
        }

        public void SetWeaponActive(bool state)
        {
            _view.BeltWeaponTransform.gameObject.SetActive(!state);
            _view.WeaponHolderTransform.gameObject.SetActive(state);
        }

        private void TakeDamage(Damage damage)
        {
            /*bool isBlocked = UnityEngine.Random.value >= 1 - someArmorDefenceChance;
            if (isBlocked) damage.Modifiers.Add(new ArmorDefenceModifier(someArmorValue));*/

            if (Health.TakeDamage(ref damage, out int resultedHealthValue))
            {
                if (resultedHealthValue <= 0)
                {
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

        private void OnStep(int legIndex)
        {
            /*int stepIndex = UnityEngine.Random.Range(0, PlayerSounds.Steps.Length);
            _sounds.Play(PlayerSounds.Steps[stepIndex]);*/
        }
    }
}
