using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/FirearmsAudio Data")]
    public class FirearmsAudioData:ScriptableObject
    {
        public AudioClip shootingAudio;
        public AudioClip reloadLeftAudio;
        public AudioClip reloadOutOfAudio;
    }
}