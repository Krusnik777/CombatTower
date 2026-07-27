namespace UnityR3ProjectTemplate.Game.EntryPoints
{
    public abstract class SceneExitParameters
    {
        public string ExitTag { get;}

        public SceneExitParameters(string exitTag)
        {
            ExitTag = exitTag;
        }

        public T As<T>() where T : SceneExitParameters
        {
            return (T)this;
        }
    }
}
