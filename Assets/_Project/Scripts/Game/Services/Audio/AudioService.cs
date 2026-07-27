using UnityEngine;
using UnityEngine.Audio;

namespace UnityR3ProjectTemplate.Game.Services
{
    public class AudioService : System.IDisposable
    {
        public MusicService Music { get; }
        public SoundService Sounds { get; }

        public AudioService()
        {
            var mixer = Resources.Load<AudioMixer>("AudioMixer");
            var audioSystemContainer = new GameObject("[AUDIO]").AddComponent<AudioListener>();
            Object.DontDestroyOnLoad(audioSystemContainer);

            Music = new MusicService(mixer, audioSystemContainer.transform);
            Sounds = new SoundService(mixer, audioSystemContainer.transform);
        }

        public void Dispose()
        {
            Music?.Dispose();
        }
    }
}
