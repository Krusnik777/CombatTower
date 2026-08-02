using DI;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class Player : System.IDisposable
    {
        private PlayerView _view;
        public Rigidbody Rigidbody => _view.Rigidbody;
        public Animator Animator => _view.Animator;
        public PlayerAvatarMovement Movement => _view.Movement;
        public AnimatorEventsCollector EventsCollector => _view.EventsCollector;

        private PlayerWeaponStateMachine _playerWeaponStateMachine;

        public Player(PlayerView view, DIContainer sceneContainer)
        {
            _view = view;

            _playerWeaponStateMachine = new(this, sceneContainer);
        }

        public void Dispose()
        {
            _playerWeaponStateMachine?.Dispose();
        }
    }
}
