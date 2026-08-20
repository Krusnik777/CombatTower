using StateMachine;
using DI;
using R3;
using System;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public abstract class ChosenWeaponState : IEnterableState
    {
        protected virtual int _weaponLayerIndex => 0;

        protected IStateMachine _parentStateMachine;
        protected DIContainer _sceneContainer;
        protected Player _player;

        protected PlayerStateMachine _playerStateMachine;

        private IDisposable _playerDeathListenerDisposable;
        private IDisposable _playerDamageListenerDisposable;

        public ChosenWeaponState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _sceneContainer = sceneContainer;

            _player = player;
            
            _playerStateMachine = new(_player, _sceneContainer);
        }

        public virtual void Enter()
        {
            _playerDeathListenerDisposable = _player.OnDeath.Subscribe(_ =>
            {
                _playerDeathListenerDisposable?.Dispose();

                _playerStateMachine.SetState<DeathState>();
            });

            _playerDamageListenerDisposable = _player.OnDamage.Subscribe(damage =>
            {
                if (_player.IsNotStaggeredByDamage) return;

                _playerStateMachine.SetState<DamageState>();
            });

            _player.Animator.SetLayerWeight(_weaponLayerIndex, 1f);

            //_playerStateMachine?.Dispose();
            _playerStateMachine.SetState<CalmMovementState>(); // TEMP ?
        }

        public virtual void Exit()
        {
            _playerDeathListenerDisposable?.Dispose();
            _playerDamageListenerDisposable?.Dispose();
            _playerStateMachine?.Dispose();
        }
    }
}
