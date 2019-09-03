using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class SoundManager : SerializedMonoBehaviour
    {
        public static SoundManager Instance;
        
        #region variables
        [FoldoutGroup("세팅")]public float BgmVolume = 0.7f;
        [FoldoutGroup("세팅")]public float SfxVolume = 0.7f;
        [SerializeField] BgmController _bgm;
        [SerializeField] SfxController _sfx;
        #endregion



        public void Awake()
        {
            Instance = this;
            
            _sfx.Setup();
        }

        void LateUpdate()
        {
            _sfx.LateUpdate();
        }



        public void PlayBgm(float volume = 1f, bool fromStart = false)
        {
            _bgm.Play(volume, fromStart);
        }
        public void StopBgm()
        {
            _bgm.StopBgm();
        }
        public void PlaySfx(SFX clip, float volume = 1f, bool isLoop = false)
        {
            _sfx.Play(clip, volume, isLoop);
        }
    }

    public enum SFX
    {
        None = 0,
        
        Ui_Ok = 100,
        Ui_Error = 101,
        Ui_MouseUp = 102,
        Ui_MouseDown = 103,
        Ui_Start = 110,
        Ui_CheckPoint = 111,

        Step_0 = 200,
        Jump_0 = 210,
        GrapItem_0 = 230,
        HitMe_0 = 240,
        HitEnemy = 250,

        DeadByFall = 300,

        Ambience_Forest_0 = 400,
    }
}