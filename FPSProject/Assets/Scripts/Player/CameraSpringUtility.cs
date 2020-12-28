using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 屏幕震动效果
/// </summary>

public class CameraSpringUtility
{
    public Vector3 value;
    //数值越大抖动越快
    private float frequence;
    //数值越大抖动后恢复正常越快
    private float damp;
    private Vector3 dampValues;
    public void UpdateSpring(float _deltalTime, Vector3 _target)
    {
        value -= _deltalTime * frequence * dampValues;
        dampValues = Vector3.Lerp(_target, value - _target, Time.deltaTime * damp);
    }
}
