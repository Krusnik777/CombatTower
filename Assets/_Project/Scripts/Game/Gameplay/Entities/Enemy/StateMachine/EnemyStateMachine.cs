using DI;
using StateMachine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class EnemyStateMachine : AbstractStateMachine
    {
        public EnemyStateMachine(Enemy enemy, DIContainer sceneContainer)
        {
            _states = new()
            {
                [typeof(IdleState)] = new IdleState(this, enemy, sceneContainer),
                [typeof(PatrolState)] = new PatrolState(this, enemy, sceneContainer),
                [typeof(ChaseState)] = new ChaseState(this, enemy, sceneContainer),
                [typeof(DamageState)] = new DamageState(this, enemy, sceneContainer),
                [typeof(DeathState)] = new DeathState(this, enemy, sceneContainer)
            };

            SetState<PatrolState>(); // TEMP
        }
    }
}
