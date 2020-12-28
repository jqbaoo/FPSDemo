using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController_CharacterController : MonoBehaviour
{

    public float walkSpeed = 5;
    public float runSpeed = 10;
    public float walkSpeedWhenCrouch = 1;
    public float runSpeedWhenCrouch = 3;

    private Animator characterAnimator;

    private CharacterController characterCtrl;
    private Rigidbody rb;
    private float moveX, moveZ;


    private float originHeight;
    private float crouchHeight = 1f;
    private Vector3 moveDirection;
    private bool isJump = false;
    private bool isCrouch = false;
    private float gravity = 9.8f;
    private float velocity;

    private float currentSpeed;
    private IEnumerator OnCrouchCoroutine;
    void Start()
    {
        //camera = GameTool.FindTheChild(this.gameObject, "Main Camera").gameObject;
        characterCtrl = transform.GetComponent<CharacterController>();
        characterAnimator = FindObjectOfType<Animator>();

        rb = transform.GetComponent<Rigidbody>();
        originHeight = characterCtrl.height;

        currentSpeed = walkSpeed;

        OnCrouchCoroutine = OnCrouch(originHeight);
    }

    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        //没有调用characterCtrl.Move()之前，characterCtrl.isGrounded是永远不会更改的，所以需要人为使用characterCtrl.Move()制造重力
        if (characterCtrl.isGrounded)
        {
            //通过计算达到斜着走也不会超过行走速度
            moveDirection.x = moveX * Mathf.Sqrt(1 - (moveZ * moveZ) / 2.0f);
            moveDirection.z = moveZ * Mathf.Sqrt(1 - (moveX * moveX) / 2.0f);
            moveDirection = transform.TransformDirection(moveDirection.x, 0, moveDirection.z);
            //跳
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = 2;
            }
            //调节高度
            if (Input.GetKeyDown(KeyCode.C))
            {
                float tmp_CurrentHeight = isCrouch ? originHeight : crouchHeight;
                //StartCoroutine(OnCrouch(tmp_CurrentHeight));
                StartOnCrouchCoroutine(tmp_CurrentHeight);
                isCrouch = !isCrouch;
            }
            //调节速度
            if (isCrouch)
            {
                currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeedWhenCrouch : walkSpeedWhenCrouch;
            }
            else
            {
                currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            }
            SetAnimation();
        }
        moveDirection.y -= gravity * Time.deltaTime;
        characterCtrl.Move(moveDirection * Time.deltaTime * currentSpeed);
    }
    private void SetAnimation()
    {
        //动画设置
        Vector3 tem_Velocity = characterCtrl.velocity;
        tem_Velocity.y = 0;
        velocity = tem_Velocity.magnitude;
        if (characterAnimator != null)
            characterAnimator.SetFloat("Velocity", velocity, 0.25f, Time.deltaTime);
    }

    private void StartOnCrouchCoroutine(float _height)
    {
        if (OnCrouchCoroutine == null)
        {
            OnCrouchCoroutine = OnCrouch(_height);
            StartCoroutine(OnCrouchCoroutine);
        }
        else
        {
            StopCoroutine(OnCrouchCoroutine);
            OnCrouchCoroutine = null;
            OnCrouchCoroutine = OnCrouch(_height);
            StartCoroutine(OnCrouchCoroutine);
        }
    }
    private IEnumerator OnCrouch(float _target)
    {
        float tmp_currentVelocity = 0;
        while (Mathf.Abs(characterCtrl.height - _target) > 0.1f)
        {
            yield return null;
            characterCtrl.height = Mathf.SmoothDamp(characterCtrl.height, _target, ref tmp_currentVelocity, Time.deltaTime * 5);
            //Debug.Log(characterCtrl.height);
        }
    }
    /// <summary>
    /// 设置动画状态机
    /// </summary>
    /// <param name="_animator"></param>
    public void SetupAnimator(Animator _animator)
    {
        characterAnimator = _animator;
    }
}
