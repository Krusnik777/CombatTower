namespace CombatTower.Game.Root.Sounds
{
    [System.Serializable]
    public struct SoundData
    {
        public string name;
        public float volume;

        public SoundData(string name, float volume)
        {
            this.name = name;
            this.volume = volume;
        }
    }
}
