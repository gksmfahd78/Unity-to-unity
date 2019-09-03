using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    [Serializable]
    public class SfxController
    {
        [SerializeField] private SfxClipsObject _sfxClips;
        [SerializeField] private AudioSource[] _sfxSources;
        [SerializeField] private int _preventOverrap;
       
        private Dictionary<SFX, int> _sfxPlayedCheck;

        private int _sourceSfxId;



        public void Setup ()
        {
            _sourceSfxId = 0;

            _sfxPlayedCheck = new Dictionary<SFX, int>();
        }

        public void LateUpdate()
        {
            if(_sfxPlayedCheck.Keys == null) return;
            
            var keys = _sfxPlayedCheck.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                _sfxPlayedCheck[keys[i]] --;
                if (_sfxPlayedCheck[keys[i]] <= 0)
                    _sfxPlayedCheck.Remove(keys[i]);
            }
        }

        public void Play(SFX clip, float volume, bool isLoop)
        {
            if (clip == SFX.None) return;
            if (_sfxPlayedCheck.ContainsKey(clip)) return;

            _sfxPlayedCheck.Add(clip, _preventOverrap);

            _sfxSources[_sourceSfxId].clip = _sfxClips.Clips[clip];
            _sfxSources[_sourceSfxId].volume = volume * SoundManager.Instance.SfxVolume;
            _sfxSources[_sourceSfxId].loop = isLoop;
            _sfxSources[_sourceSfxId].Play();

            MoveIndexToNext(_sourceSfxId);
        }

        private void MoveIndexToNext(int from)
        {
            _sourceSfxId++;
            _sourceSfxId = _sourceSfxId % _sfxSources.Length;

            if (_sfxSources[_sourceSfxId].isPlaying && _sourceSfxId != from)
                MoveIndexToNext(from);
        }
    }


    [Serializable]
    public class BgmController
    {
        [SerializeField] private AudioClip _bgmClip;
        [SerializeField] private AudioSource _bgmSource;

        

        public void Play(float volume, bool fromStart = false)
        {
            if(_bgmSource.clip == _bgmClip && !fromStart) return;
            
            _bgmSource.clip = _bgmClip;
            _bgmSource.volume = volume * SoundManager.Instance.BgmVolume;
            _bgmSource.loop = true;
            _bgmSource.Play();

            if(fromStart)
                _bgmSource.time = 0f;
        }

        public void StopBgm()
        {
            _bgmSource.Stop();
        }
    }
}