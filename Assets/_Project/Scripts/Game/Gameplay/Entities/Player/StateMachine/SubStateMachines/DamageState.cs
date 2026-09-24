using System;
using DI;
using StateMachine;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class DamageState : IEnterableState<HealthSystem.Damage>
    {
        private const string _getHitTrigger = "GetHit";
        private const string _hitTypeInt = "HitType";
        private readonly int[] _frontSimpleHits = new[]{ 0, 1, 2 };
        private readonly int[] _frontHardHits = new[]{ 2 };
        private readonly int[] _backSimpleHits = new[]{ 3, 4 };
        private readonly int[] _backHardHits = new[]{ 4 };

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;
        //private GameInputService _gameInputService;

        private IDisposable _hitRecoveryListenerDisposable;

        public DamageState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;

            //_gameInputService = _sceneContainer.Resolve<GameInputService>();
        }

        public void Enter(HealthSystem.Damage damage)
        {
            DisposeOfListeners();

            _hitRecoveryListenerDisposable = _player.EventsCollector.OnRecoveryAfterHit.Subscribe(_ =>
            {
                DisposeOfListeners();

                _parentStateMachine.SetState<BattleState, BattleState.EntryTag>(BattleState.EntryTag.Movement);
            });

            _player.Movement.IsControlledByRootMotion = true;

            int hitIndex = 0;

            if (damage.Direction == HealthSystem.DamageDirection.Back)
            {
                hitIndex = _backSimpleHits[UnityEngine.Random.Range(0, _backSimpleHits.Length)];
            }
            else
            {
                hitIndex = _frontSimpleHits[UnityEngine.Random.Range(0, _frontSimpleHits.Length)];
            }

            _player.Animator.SetInteger(_hitTypeInt, hitIndex);
            _player.Animator.SetTrigger(_getHitTrigger);
        }

        public void Exit()
        {
            DisposeOfListeners();

            _player.Movement.IsControlledByRootMotion = false;
        }

        private void DisposeOfListeners()
        {
            _hitRecoveryListenerDisposable?.Dispose();
        }
    }
}
