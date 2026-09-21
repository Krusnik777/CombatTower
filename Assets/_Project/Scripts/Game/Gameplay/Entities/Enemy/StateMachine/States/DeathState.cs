using DI;
using StateMachine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class DeathState : IEnterableState
    {
        private const string _dieTrigger = "Die";
        private const string _deathTypeInt = "DeathType";
        private const int _deathTypesAmount = 2;

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Enemy _enemy;

        public DeathState(IStateMachine parentStateMachine, Enemy enemy, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _enemy = enemy;
            _sceneContainer = sceneContainer;
        }

        public void Enter()
        {
            DisposeOfListeners();

            _enemy.Animator.SetInteger(_deathTypeInt, UnityEngine.Random.Range(0, _deathTypesAmount));
            _enemy.Animator.SetTrigger(_dieTrigger);
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
