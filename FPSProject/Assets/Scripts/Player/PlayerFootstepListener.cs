using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstepListener : MonoBehaviour
{
    public FootstepAudioData footstepAudioData;
    public AudioSource audioSource;
    public LayerMask layerMask;

    private CharacterController characterCtrl;
    private FPController_CharacterController fPController_CharacterController;

    private float delayPlayTime;
    private PlayerMoveState playerMoveState = PlayerMoveState.Walk;
    void Start()
    {
        characterCtrl = transform.GetComponent<CharacterController>();
        fPController_CharacterController = transform.GetComponent<FPController_CharacterController>();
    }
    void FixedUpdate()
    {
        RaycastHit rayHit;
        if (characterCtrl.isGrounded)
        {
            if (characterCtrl.velocity.normalized.magnitude >= 0.1f)
            {
                delayPlayTime += Time.deltaTime;

                if (characterCtrl.velocity.magnitude >= fPController_CharacterController.walkSpeed + 1f)
                {
                    playerMoveState = PlayerMoveState.Run;
                }
                else  
                {
                    playerMoveState = PlayerMoveState.Walk;
                }

                //检测地面类型
                bool tmp_isHit = Physics.Linecast(this.transform.position, this.transform.position + Vector3.down * (characterCtrl.height / 2 + characterCtrl.skinWidth - characterCtrl.center.y), out rayHit, layerMask);

                if (tmp_isHit)
                {
                    //播放声音
                    foreach (FootstepAudio tmp_Footstepaudio in footstepAudioData.footstepAudios)
                    {
                        if (rayHit.collider.CompareTag(tmp_Footstepaudio.tag))
                        {
                            float tmp_DelayPlayTime = 0;
                            switch (playerMoveState)
                            {
                                case PlayerMoveState.Walk:
                                    tmp_DelayPlayTime = tmp_Footstepaudio.walkDelay;
                                    break;
                                case PlayerMoveState.Run:
                                    tmp_DelayPlayTime = tmp_Footstepaudio.runDelay;
                                    break;
                                case PlayerMoveState.WalkWhenCrouch:
                                    tmp_DelayPlayTime = tmp_Footstepaudio.walkWhenCrouchDelay;
                                    break;
                                case PlayerMoveState.RunWhenCrouch:
                                    tmp_DelayPlayTime = tmp_Footstepaudio.runWhenCrouchDelay;
                                    break;
                                default:
                                    break;
                            }
                            if (delayPlayTime >= tmp_DelayPlayTime)
                            {
                                int tmp_ClipCount = tmp_Footstepaudio.audioClip.Count;
                                int tmp_ClipIndex = Random.Range(0, tmp_ClipCount);
                                audioSource.clip = tmp_Footstepaudio.audioClip[tmp_ClipIndex];
                                audioSource.Play();
                                delayPlayTime = 0;
                                break;
                            }
                        }
                    }

                }
            }
        }
    }
}
public enum PlayerMoveState
{
    Walk,
    Run,
    WalkWhenCrouch,
    RunWhenCrouch,
}