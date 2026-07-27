using CombatTower.Game.Root;

namespace CombatTower.Game.EntryPoints
{
    public class GameplayEnterParameters : SceneEnterParameters
    {
        public int Runs { get; }
        
        public GameplayEnterParameters(int currentRun = 0) : base(Scenes.GAMEPLAY)
        {
            Runs = currentRun;
        }
    }
}
