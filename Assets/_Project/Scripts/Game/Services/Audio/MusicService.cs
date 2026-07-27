using System;
using R3;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityR3ProjectTemplate.Game.Services
{
    public class MusicService : IDisposable
    {
        private const string _musicPath = "Music";
        private const string _calmTrackName = "singularity_calm";
        private const string _actionTrackName = "singularity_action";

        private const float _defaultAudioSourceVolume = 0.15f;
        private const float _volumeChangeSpeed = 2f;

        private AudioSource _calmSource;
        private AudioSource _actionSource;

        private float _targetState;
        private float _currentState;

        public MusicService(AudioMixer mixer, Transform audioSystemTransform)
        {
            var bgmGroup = mixer.FindMatchingGroups("BGM")[0];

            var bgmContainer = new GameObject("[BACKGROUND_MUSIC]");
            bgmContainer.transform.SetParent(audioSystemTransform);

            _calmSource = bgmContainer.AddComponent<AudioSource>();
            _calmSource.outputAudioMixerGroup = bgmGroup;
            _calmSource.playOnAwake = false;
            _calmSource.loop = true;
            _calmSource.volume = _defaultAudioSourceVolume;
            _calmSource.clip = Resources.Load<AudioClip>($"{_musicPath}/{_calmTrackName}");

            _actionSource = bgmContainer.AddComponent<AudioSource>();
            _actionSource.outputAudioMixerGroup = bgmGroup;
            _actionSource.playOnAwake = false;
            _actionSource.loop = true;
            _actionSource.volume = _defaultAudioSourceVolume;
            _actionSource.clip = Resources.Load<AudioClip>($"{_musicPath}/{_actionTrackName}");
        }

        private IDisposable _volumeUpdateDisposable;

        public void Dispose()
        {
            _volumeUpdateDisposable?.Dispose();
        }

        public void Play(bool isActionState)
        {
            _targetState = isActionState ? 1f : 0f;

            if (!_calmSource.isPlaying || !_actionSource.isPlaying)
            {
                _calmSource.volume = isActionState ? 0f : _defaultAudioSourceVolume;
                _actionSource.volume = isActionState ? _defaultAudioSourceVolume : 1f;

                _calmSource.Play();
                _actionSource.Play();
            }

            _volumeUpdateDisposable?.Dispose();

            _volumeUpdateDisposable = Observable.EveryUpdate().Subscribe(_ => UpdateVolumes());
        }

        public void Stop()
        {
            _volumeUpdateDisposable?.Dispose();

            if (_calmSource == null || _actionSource == null) return;

            _calmSource.Stop();
            _actionSource.Stop();
        }

        private void UpdateVolumes()
        {
            if (_calmSource == null || _actionSource == null)
            {
                _volumeUpdateDisposable?.Dispose();
                return;
            }

            _currentState = Mathf.Lerp(_currentState, _targetState, Time.deltaTime * _volumeChangeSpeed);

            _calmSource.volume = Mathf.Lerp(0f, _defaultAudioSourceVolume, 1 - _currentState);
            _actionSource.volume = Mathf.Lerp(0f, _defaultAudioSourceVolume, _currentState);

            if (_currentState == _targetState) _volumeUpdateDisposable?.Dispose();
        }
    }
}
