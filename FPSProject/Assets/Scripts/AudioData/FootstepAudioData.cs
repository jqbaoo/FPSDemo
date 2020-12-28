using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/Footstep Audio Data")] 
public class FootstepAudioData : ScriptableObject
{
    public List<FootstepAudio> footstepAudios = new List<FootstepAudio>();
}
[System.Serializable]
public class FootstepAudio
{
    public string tag;
    public List<AudioClip> audioClip = new List<AudioClip>();
    public float walkDelay;
    public float runDelay;
    public float walkWhenCrouchDelay;
    public float runWhenCrouchDelay;
}