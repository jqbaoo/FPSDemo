using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPMovement : MonoBehaviour
{
    private float mouseX;
    private float mouseY;

    private Vector3 cameraTransform = Vector3.one;
    public int mouseXSensitivity = 1;
    public int mouseYSensitivity = 1;
    public Vector2 axisYLimit;

    private GameObject camera;

    private Rigidbody rb;

    public float moveSensitivity = 10f;
    void Start()
    {
        camera = GameTool.FindTheChild(this.gameObject, "Main Camera").gameObject;
        rb = this.transform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //鼠标左移改变的是Y轴
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        MouseLook();
    }
    void FixedUpdate()
    {
        Movement();
    }
    /// <summary>
    /// 人物移动
    /// </summary>
    private void Movement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        this.transform.rotation = Quaternion.Euler(0, cameraTransform.y, 0);

        Vector3 currentDirection = new Vector3(moveX, 0, moveZ);
        currentDirection = transform.TransformDirection(currentDirection);
        currentDirection *= moveSensitivity;

        //实时获取速度，相减的目的是控制速度，否则添加的力永远不会停下
        Vector3 currentVelocity = rb.velocity;
        Vector3 velocityChange = currentDirection - currentVelocity;
        velocityChange.y = 0;
        rb.AddForce(velocityChange, ForceMode.VelocityChange);        
    }
    /// <summary>
    /// 鼠标转动
    /// </summary>
    private void MouseLook()
    {
        cameraTransform.x -= mouseY * mouseXSensitivity;
        cameraTransform.y += mouseX * mouseYSensitivity;

        cameraTransform.x = Mathf.Clamp(cameraTransform.x, axisYLimit.x, axisYLimit.y);

        camera.transform.rotation = Quaternion.Euler(cameraTransform.x, cameraTransform.y, 0);
    }
}
