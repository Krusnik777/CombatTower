namespace CombatTower.Game.EntryPoints
{
    public class GameplayExitParameters : SceneExitParameters
    {
        public int Runs { get; }

        public GameplayExitParameters(string exitTag, int currentRuns) : base(exitTag)
        {
            Runs = currentRuns;
        }
    }
}
