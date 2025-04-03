using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Audio;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Utils.Extensions;
using UnityEngine;

namespace Game.Runtime.Services.Audio
{
    public class AudioService : IService, IDisposable
    {
        public bool MuteSFX;
        public bool MuteAmbient;
        public bool MuteMusic;

        public float SFXVolume = 1.0f;
        public float AmbientVolume = 1.0f;
        public float MusicVolume = 1.0f;
        
        private readonly GameObject _audioObject;
        private readonly AudioSource _ambientSource;
        private readonly AudioSource _musicSource;

        private readonly Dictionary<string, float> _lastPlayTime;
        private readonly List<AudioSource> _audioSourcePool;

        private CancellationTokenSource _poolTokenSource;

        public AudioService(int initialPoolSize)
        {
            _lastPlayTime = new Dictionary<string, float>();
            _audioSourcePool = new List<AudioSource>();
            
            _audioObject = new GameObject(nameof(AudioService));
            _ambientSource = _audioObject.AddComponent<AudioSource>();
            _musicSource = _audioObject.AddComponent<AudioSource>();

            for (int i = 0; i < initialPoolSize; i++)
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
        
        public void Play<T>() where T : CMSEntity
        {
            Play(CMSProvider.GetEntity<T>());
        }

        public void Play(CMSEntity definition)
        {
            if (definition.Is<SFXComponent>())
                PlaySFX(definition);
            else if (definition.Is<AmbientComponent>(out var ambient))
                PlayAmbient(ambient);
            else if (definition.Is<MusicComponent>(out var music))
                PlayMusic(music);
        }

        public void PlaySFX(CMSEntity sfxEntity)
        {
            if (!MuteSFX && CanPlaySFX(sfxEntity.EntityId))
            {
                if (sfxEntity.Is(out SFXLibraryComponent sfxLibraryComponent))
                {
                    var clip = sfxLibraryComponent.Clips.GetRandom(ignoreEmpty: true);
                    var audioSource = GetAvailableAudioSource();

                    if (audioSource != null)
                    {
                        audioSource.clip = clip;
                        audioSource.volume = SFXVolume * sfxLibraryComponent.Volume;
                        
                        audioSource.Play();
                        _lastPlayTime[sfxEntity.EntityId] = Time.time;
                        
                        ReturnAudioSourceToPool(audioSource, clip.length).Forget();
                    }
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
            _poolTokenSource = new CancellationTokenSource();
            await UniTask.WaitForSeconds(delay, cancellationToken: _poolTokenSource.Token);
            
            audioSource.Stop();
            audioSource.clip = null;
        }

        private bool CanPlaySFX(string sfxId)
        {
            if (!_lastPlayTime.TryGetValue(sfxId, value: out var lastTimePlayed))
                return true;

            float cooldown = CMSProvider.GetEntity<CMSEntity>().GetComponent<SFXComponent>().Cooldown;
            return Time.time - lastTimePlayed >= cooldown;
        }

        public void PlayAmbient(AmbientComponent ambient)
        {
            if (!MuteAmbient)
            {
                _ambientSource.clip = ambient.Clip;
                _ambientSource.loop = true;
                _ambientSource.volume = AmbientVolume;
                
                _ambientSource.Play();
            }
        }

        public void PlayMusic(MusicComponent music)
        {
            if (!MuteMusic)
            {
                _musicSource.clip = music.Clip;
                _musicSource.loop = true;
                _musicSource.volume = MusicVolume;
                
                _musicSource.Play();
            }
        }

        public void SetVolume(AudioType type, float volume)
        {
            switch (type)
            {
                case AudioType.SFX:
                    SFXVolume = volume;
                    break;
                case AudioType.Ambient:
                    AmbientVolume = volume;
                    _ambientSource.volume = AmbientVolume;
                    break;
                case AudioType.Music:
                    MusicVolume = volume;
                    _musicSource.volume = volume;
                    break;
            }
        }

        public void Mute(AudioType type, bool mute)
        {
            switch (type)
            {
                case AudioType.SFX:
                    MuteSFX = mute;
                    break;
                case AudioType.Ambient:
                    MuteAmbient = mute;
                    _ambientSource.enabled = !mute;
                    break;
                case AudioType.Music:
                    MuteMusic = mute;
                    _musicSource.enabled = !mute;
                    break;
            }
        }

        public void Dispose()
        {
            _poolTokenSource?.Cancel();
            _poolTokenSource?.Dispose();
            
            UnityEngine.Object.Destroy(_audioObject);
        }
    }
}