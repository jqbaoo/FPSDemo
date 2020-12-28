using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPMouseLook : MonoBehaviour
{
    public Vector2 axisYLimit;
    public AnimationCurve recoilCurve;
    public Vector2 recoilRange;

    private float currentRecoilTime;
    private Vector2 currentRecoil;
    public float recoilFadeOutTime = 0.3f;

    private Vector3 cameraTransform = Vector3.one;
    private int mouseXSensitivity = 1;
    private int mouseYSensitivity = 1;
    [SerializeField]
    private GameObject camera;

    void Start()
    {
        currentRecoil = recoilRange;
    }

    private bool IsAllowMoveCamera = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            IsAllowMoveCamera = !IsAllowMoveCamera;
        }
        if (IsAllowMoveCamera)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            MouseLook();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void MouseLook()
    {
        //鼠标左移改变的是Y轴
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        cameraTransform.x -= mouseY * mouseXSensitivity;
        cameraTransform.y += mouseX * mouseYSensitivity;

        CalculateRecoilOffset();
        //Debug.Log(currentRecoil);
        //添加枪口抖动
        cameraTransform.x -= currentRecoil.x;
        cameraTransform.y += currentRecoil.y;

        cameraTransform.x = Mathf.Clamp(cameraTransform.x, axisYLimit.x, axisYLimit.y);

        camera.transform.rotation = Quaternion.Euler(cameraTransform.x, cameraTransform.y, 0);
        this.transform.rotation = Quaternion.Euler(0, cameraTransform.y, 0);
    }
    
    private void CalculateRecoilOffset()
    {
        currentRecoilTime += Time.deltaTime;
        float tmp_RecoilFraction = currentRecoilTime / recoilFadeOutTime;
        float tmp_RecoilValue = recoilCurve.Evaluate(tmp_RecoilFraction);
        //Debug.Log(currentRecoilTime);
        currentRecoil = Vector2.Lerp(Vector2.zero, currentRecoil, tmp_RecoilValue);       
    }
    public void CollimationOffset() 
    {
        currentRecoil += recoilRange;
        currentRecoilTime = 0;
    }
}
