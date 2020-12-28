using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Weapon;

public class CrossHairUI : MonoBehaviour
{
    public RectTransform rectTransform;
    public CharacterController characterController;

    public float originalSize;
    public float maxSize;
    private float currentSize;
    void Update()
    {
        currentSize = Mathf.Lerp(currentSize, characterController.velocity.magnitude * 5 + 100, Time.deltaTime * 5);

        rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }
    public void SetupSight()
    {
        currentSize = Mathf.Lerp(currentSize, characterController.velocity.magnitude * 5 + 100, Time.deltaTime * 5);

        rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }
}
