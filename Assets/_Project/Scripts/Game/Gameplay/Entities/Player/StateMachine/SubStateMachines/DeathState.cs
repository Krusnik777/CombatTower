using StateMachine;
using DI;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class DeathState : IEnterableState
    {
        private const string _dieTrigger = "Die";
        private const string _deathTypeInt = "DeathType";
        private const int _deathTypesAmount = 2;

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;
        //private GameInputService _gameInputService;
        private LockOnHandler _lockOnHandler;
        private CameraRotation _cameraRotation;

        public DeathState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;

            //_gameInputService = _sceneContainer.Resolve<GameInputService>();
            _lockOnHandler = _sceneContainer.Resolve<LockOnHandler>();
            _cameraRotation = _sceneContainer.Resolve<CameraRotation>();
        }

        public void Enter()
        {
            DisposeOfListeners();

            _lockOnHandler.ResetTargetAndDisableInputListener();
            _cameraRotation.Bind(null);

            _player.Movement.IsControlledByRootMotion = true;
            _player.Animator.SetInteger(_deathTypeInt, UnityEngine.Random.Range(0, _deathTypesAmount));
            _player.Animator.SetTrigger(_dieTrigger);
        }

        public void Exit()
        {
            DisposeOfListeners();

            _player.Movement.IsControlledByRootMotion = false;
        }

        private void DisposeOfListeners()
        {
            
        }
    }
}
