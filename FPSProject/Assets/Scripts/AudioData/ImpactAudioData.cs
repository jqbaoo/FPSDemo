using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Impact Audio Data")]
    public class ImpactAudioData : ScriptableObject
    {
        public List<ImpactTagWithAudio> impactTagWithAudio = new List<ImpactTagWithAudio>();
    }
    [System.Serializable]
    public class ImpactTagWithAudio
    {
        public string tag;
        public List<AudioClip> impactAudioClips = new List<AudioClip>();
    }
}