using DI;
using StateMachine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class IdleState : IEnterableState
    {
        protected const string _forwardMoveFloat = "ForwardMove";
        protected const string _sidewardMoveFloat = "SidewardMove";
        protected const float _movementThreshold = 0.05f;

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Enemy _enemy;

        public IdleState(IStateMachine parentStateMachine, Enemy enemy, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _enemy = enemy;
            _sceneContainer = sceneContainer;
        }

        public void Enter()
        {
            DisposeOfListeners();

            _enemy.Animator.SetFloat(_forwardMoveFloat, 0f);
            _enemy.Animator.SetFloat(_sidewardMoveFloat, 0f);
        }

        public void Exit()
        {
            DisposeOfListeners();
        }

        private void DisposeOfListeners()
        {

        }
    }
}
