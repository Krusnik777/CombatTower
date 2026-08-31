namespace CombatTower.Game.Settings
{
    public interface ISettingsProvider
    {
        public ApplicationSettings ApplicationSettings { get; }
        public GameSettings GameSettings { get; }
    }
}
