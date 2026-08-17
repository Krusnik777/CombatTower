using CombatTower.Game.Services;
using DI;
using StateMachine;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class PlayerStateMachine : AbstractStateMachine
    {
        private System.IDisposable _damageTestDisposable; // TEMP

        public PlayerStateMachine(Player player, DIContainer sceneContainer)
        {
            _states = new()
            {
                [typeof(MovementState)] = new MovementState(this, player, sceneContainer),
                [typeof(CalmState)] = new CalmState(this, player, sceneContainer),
                [typeof(BattleState)] = new BattleState(this, player, sceneContainer),
                [typeof(DodgeState)] = new DodgeState(this, player, sceneContainer),
                [typeof(GuardState)] = new GuardState(this, player, sceneContainer),
                [typeof(DamageState)] = new DamageState(this, player, sceneContainer),
                [typeof(DeathState)] = new DeathState(this, player, sceneContainer)
            };

            // TEMP

            var gameInputService = sceneContainer.Resolve<GameInputService>();
            _damageTestDisposable = gameInputService.OnTestButtonPressed?.Subscribe(_ =>
            {
                /*if (_currentState != null && _currentState is DeathState) return;

                SetState<DeathState>();*/
            });  
        }

        public override void Dispose()
        {
            base.Dispose();

            _damageTestDisposable?.Dispose();  // TEMP
        }
    }
}
