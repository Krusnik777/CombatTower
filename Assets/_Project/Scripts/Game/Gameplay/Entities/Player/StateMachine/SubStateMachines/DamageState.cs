using System;
using DI;
using StateMachine;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class DamageState : IEnterableState
    {
        private const string _getHitTrigger = "GetHit";
        private const string _hitTypeInt = "HitType";
        private const int _hitTypesAmount = 5;

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

        public void Enter()
        {
            DisposeOfListeners();

            _hitRecoveryListenerDisposable = _player.EventsCollector.OnRecoveryAfterHit.Subscribe(_ =>
            {
                DisposeOfListeners();

                _parentStateMachine.SetState<BattleState, BattleState.EntryTag>(BattleState.EntryTag.Movement);
            });

            _player.Movement.IsControlledByRootMotion = true;
            _player.Animator.SetInteger(_hitTypeInt, UnityEngine.Random.Range(0, _hitTypesAmount));
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
