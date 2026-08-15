using CombatTower.Game.Configs;
using DI;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class Player : System.IDisposable
    {
        public PlayerParametersConfig ParametersConfig { get; }

        private PlayerView _view;
        public Rigidbody Rigidbody => _view.Rigidbody;
        public Animator Animator => _view.Animator;
        public PlayerAvatarMovement Movement => _view.Movement;
        public AnimatorEventsCollector EventsCollector => _view.EventsCollector;

        private PlayerWeaponStateMachine _playerWeaponStateMachine;

        public Player(PlayerParametersConfig config, PlayerView view, DIContainer sceneContainer)
        {
            ParametersConfig = config;

            _view = view;
            
            _playerWeaponStateMachine = new(this, sceneContainer);
        }

        public void Dispose()
        {
            _playerWeaponStateMachine?.Dispose();
        }
    }
}
