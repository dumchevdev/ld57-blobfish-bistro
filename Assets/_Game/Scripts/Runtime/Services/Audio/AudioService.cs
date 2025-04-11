using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Audio;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Utils.Extensions;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Audio
{
    public class AudioService : IService, IDisposable
    {
        private bool _muteSFX;
        private bool _muteAmbient;
        private bool _muteMusic;

        private float _sfxVolume = 1.0f;
        private float _ambientVolume = 1.0f;
        private float _musicVolume = 1.0f;
        
        private readonly GameObject _audioObject;
        private readonly AudioSource _ambientSource;
        private readonly AudioSource _musicSource;

        private readonly Dictionary<string, float> _lastPlayTime;
        private readonly List<AudioSource> _audioSourcePool;

        private CancellationTokenSource _poolTokenSource;

        public AudioService()
        {
            _lastPlayTime = new Dictionary<string, float>();
            _audioSourcePool = new List<AudioSource>();
            
            _audioObject = new GameObject(nameof(AudioService));
            _ambientSource = _audioObject.AddComponent<AudioSource>();
            _musicSource = _audioObject.AddComponent<AudioSource>();

            for (int i = 0; i < 10; i++)
            {
                var audioSource = _audioObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;

                _audioSourcePool.Add(audioSource);
            }
            
            UnityEngine.Object.DontDestroyOnLoad(_audioObject);
        }

        public void Play(string entityId)
        {
            Play(CMSProvider.GetEntity(entityId));
        }
        
        public void SetVolume(AudioType type, float volume)
        {
            switch (type)
            {
                case AudioType.SFX:
                    _sfxVolume = volume;
                    break;
                case AudioType.Ambient:
                    _ambientVolume = volume;
                    _ambientSource.volume = _ambientVolume;
                    break;
                case AudioType.Music:
                    _musicVolume = volume;
                    _musicSource.volume = volume;
                    break;
            }
        }

        public void Mute(AudioType type, bool mute)
        {
            switch (type)
            {
                case AudioType.SFX:
                    _muteSFX = mute;
                    break;
                case AudioType.Ambient:
                    _muteAmbient = mute;
                    _ambientSource.enabled = !mute;
                    break;
                case AudioType.Music:
                    _muteMusic = mute;
                    _musicSource.enabled = !mute;
                    break;
            }
        }

        private void Play(CMSEntity definition)
        {
            if (definition.Is(out SFXComponent sfxComponent))
                PlaySFX(definition, sfxComponent);
            else if (definition.Is(out AmbientComponent ambientComponent))
                PlayAmbient(ambientComponent);
            else if (definition.Is(out MusicComponent musicComponent))
                PlayMusic(musicComponent);
        }

        private void PlaySFX(CMSEntity sfxEntity, SFXComponent sfxComponent)
        {
            if (!_muteSFX && CanPlaySFX(sfxEntity))
            {
                var clip = sfxComponent.Clips.GetRandom(ignoreEmpty: true);
                var audioSource = GetAvailableAudioSource();

                if (audioSource != null)
                {
                    audioSource.clip = clip;
                    audioSource.volume = _sfxVolume * sfxComponent.Volume;
                    
                    audioSource.Play();
                    _lastPlayTime[sfxEntity.EntityId] = Time.time;
                    
                   ReturnAudioSourceToPool(audioSource, clip.length).Forget();
                }
            }
        }

        private AudioSource GetAvailableAudioSource()
        {
            foreach (var audioSource in _audioSourcePool)
            {
                if (!audioSource.isPlaying)
                {
                    return audioSource;
                }
            }

            var newAudioSource = _audioObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            
            _audioSourcePool.Add(newAudioSource);
            return newAudioSource;
        }

        private async UniTask ReturnAudioSourceToPool(AudioSource audioSource, float delay)
        {
            _poolTokenSource?.Cancel();
            _poolTokenSource = new CancellationTokenSource();

            try
            {
                await UniTask.WaitForSeconds(delay, cancellationToken: _poolTokenSource.Token)
                    .SuppressCancellationThrow();
            
                if ( _poolTokenSource == null || _poolTokenSource.IsCancellationRequested)
                    return;
                
                audioSource.Stop();
                audioSource.clip = null;
            }
            finally
            {
                ResetPoolToken();
            }
        }

        private bool CanPlaySFX(CMSEntity entity)
        {
            if (!_lastPlayTime.TryGetValue(entity.EntityId, value: out var lastTimePlayed))
                return true;

            float cooldown = entity.GetComponent<SFXComponent>().Cooldown;
            return Time.time - lastTimePlayed >= cooldown;
        }

        private void PlayAmbient(AmbientComponent ambient)
        {
            if (!_muteAmbient)
            {
                _ambientSource.clip = ambient.Clip;
                _ambientSource.loop = true;
                _ambientSource.volume = _ambientVolume;
                
                _ambientSource.Play();
            }
        }

        private void PlayMusic(MusicComponent music)
        {
            if (!_muteMusic)
            {
                _musicSource.clip = music.Clip;
                _musicSource.loop = true;
                _musicSource.volume = _musicVolume;
                
                _musicSource.Play();
            }
        }

        private void ResetPoolToken()
        {
            _poolTokenSource?.Dispose();
            _poolTokenSource = null;
        }
        
        public void Dispose()
        {
            ResetPoolToken();
        }
    }
}