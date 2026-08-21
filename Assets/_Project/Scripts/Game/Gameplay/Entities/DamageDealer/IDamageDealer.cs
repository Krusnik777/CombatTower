namespace CombatTower.Game.Gameplay.Entities
{
    public interface IDamageDealer : System.IDisposable
    {
        public void SubscribeToAttack(System.Action onAttack);
    }
}
