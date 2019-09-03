using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    [CreateAssetMenu]
    public class SfxClipsObject : SerializedScriptableObject
    {
        public Dictionary<SFX, AudioClip> Clips;
    }
}