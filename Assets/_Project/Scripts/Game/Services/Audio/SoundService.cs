using System.Collections.Generic;
using CombatTower.Game.Root.Sounds;
using UnityEngine;
using UnityEngine.Audio;

namespace CombatTower.Game.Services
{
    public class SoundService
    {
        private const string _soundsPath = "Sounds";

        private AudioSource _audioSource;

        private Dictionary<string, AudioClip> _cachedSoundsMap;

        public SoundService(AudioMixer mixer, Transform audioSystemTransform)
        {
            var sfxGroup = mixer.FindMatchingGroups("SFX")[0];

            _audioSource = new GameObject("[SOUNDS]").AddComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = sfxGroup;
            _audioSource.playOnAwake = false;
            _audioSource.transform.SetParent(audioSystemTransform);

            //var loopSoundsContainer = new GameObject("[SOUNDS_LOOP]").AddComponent<AudioSource>();
            //loopSoundsContainer.outputAudioMixerGroup = sfxGroup;
            //loopSoundsContainer.transform.SetParent(audioSystemTransform);

            _cachedSoundsMap = new();
        }

        public void Play(SoundData data, bool addToCache = true) => Play(data.name, data.volume, addToCache);

        public void Play(string soundName, float volume = 1f, bool addToCache = true)
        {
            AudioClip clip;

            var isCached = _cachedSoundsMap.ContainsKey(soundName);

            if (isCached)
            {
                clip = _cachedSoundsMap[soundName];
            }
            else
            {
                clip = Resources.Load<AudioClip>($"{_soundsPath}/{soundName}");

                if (clip == null)
                {
                    Debug.LogWarning($"Sound not found: {soundName}");
                    return;
                }

                if (addToCache) _cachedSoundsMap.Add(soundName, clip);
            }

            _audioSource.PlayOneShot(clip, volume);
        }
    }
}
