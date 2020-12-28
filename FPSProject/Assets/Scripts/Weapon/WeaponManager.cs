using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Items;

namespace Scripts.Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        public Firearms mainWeapon;
        public Firearms secondWeapon;

        public Transform worldCameraTransform;
        /// <summary>
        /// 拾取物品的最长距离
        /// </summary>
        public float rayCastMaxDistance = 2;
        /// <summary>
        /// 识别物体的层级
        /// </summary>
        public LayerMask checkItemLayerMask;
        public List<Firearms> armsList = new List<Firearms>();

        public CrossHairUI crossHairUI;
        public PlayerInfoUI palyerInfoUI;

        internal Firearms carryWeapon;
        private bool isAim = false;
        private FPController_CharacterController characterController;

        private IEnumerator waitingForHolsterEnd;
        void Start()
        {
            if (mainWeapon)
            {
                carryWeapon = mainWeapon;
            }
            characterController = FindObjectOfType<FPController_CharacterController>();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                CheckItem();
            }

            //没有武器就执行不了下面的枪支操作
            if (!carryWeapon) return;

            SwapWeapon();
            //开枪
            if (Input.GetMouseButton(0))
            {
                if (carryWeapon == mainWeapon)
                {
                    carryWeapon.DoAttack();
                }
            }
            //单点
            if (Input.GetMouseButtonDown(0))
            {
                if (carryWeapon == secondWeapon)
                {
                    carryWeapon.DoAttack();
                }
            }
            //换弹
            if (Input.GetKeyDown(KeyCode.R))
            {
                carryWeapon.DoReload();
            }
            //瞄准
            if (Input.GetMouseButtonDown(1))
            {
                
                isAim = !isAim;
                carryWeapon.DoAim(isAim);

                if (isAim)
                {
                    crossHairUI.gameObject.SetActive(false);
                }
                else
                {
                    crossHairUI.gameObject.SetActive(true);
                }
            }
            palyerInfoUI.UpdateUI(carryWeapon.GetCurrentAmmo, carryWeapon.GetCurrentMaxAmmoCarried);
        }
        private void CheckItem()
        {
            RaycastHit tmp_rayHit;
            bool tmp_IsItem = Physics.Raycast(worldCameraTransform.position, worldCameraTransform.forward * 10, out tmp_rayHit, rayCastMaxDistance, checkItemLayerMask);
            if (tmp_IsItem)
            {
                //Debug.Log(tmp_rayHit.collider.name);
                BaseItem tmp_BaseItem = tmp_rayHit.transform.GetComponent<BaseItem>();          
                //判断是否枪械类物品
                if (tmp_BaseItem is FirearmsItem)
                {
                    PickupWeapon(tmp_BaseItem);
                }
                else if (tmp_BaseItem is AttachmentItem)
                {
                    if (!carryWeapon) return;
                    PickupAttachment(tmp_BaseItem);
                }
            }

            //Debug.DrawRay(worldCameraTransform.position, worldCameraTransform.forward * 10, Color.red, Time.deltaTime);
            
        }

        private void PickupWeapon(BaseItem _baseItem)
        {
            FirearmsItem tmp_firearmsItem = _baseItem as FirearmsItem;
            //判断枪对应的手臂
            foreach (Firearms tmp_Arm in armsList)
            {
                if (tmp_firearmsItem.armsName.CompareTo(tmp_Arm.name) != 0) continue;

                switch (tmp_firearmsItem.currentFirearmsType)
                {
                    case FirearmsItem.FirearmsType.AssultRefile:
                        mainWeapon = tmp_Arm;
                        break;
                    case FirearmsItem.FirearmsType.HandGun:
                        secondWeapon = tmp_Arm;
                        break;
                    default:
                        break;
                }
                SetupCarriedWeapon(tmp_Arm);

            }
        }
        private void PickupAttachment(BaseItem _baseItem)
        {
            AttachmentItem tmp_AttachmentItem = _baseItem as AttachmentItem;

            switch (tmp_AttachmentItem.currentAttachmentType)
            {
                case AttachmentItem.AttachmentType.Scope:
                    //遍历查找当前武器上倍镜是否有捡起来的倍镜
                    foreach (ScopeInfo tmp_ScopeInfo in carryWeapon.scopeInfo)
                    {
                        //遍历到的倍镜与拾取的倍镜不匹配
                        if (tmp_ScopeInfo.scopeName.CompareTo(tmp_AttachmentItem.itemName) != 0)
                        {
                            tmp_ScopeInfo.ScopeGameObject.SetActive(false);
                            carryWeapon.baseIronSight.ScopeGameObject.SetActive(true);
                            carryWeapon.SetupCarriedScope(null);
                            continue;
                        }
                        tmp_ScopeInfo.ScopeGameObject.SetActive(true);
                        carryWeapon.baseIronSight.ScopeGameObject.SetActive(false);
                        carryWeapon.SetupCarriedScope(tmp_ScopeInfo);
                        //匹配到倍镜后跳出Foreach循环
                        //break;
                    }
                    break;
                case AttachmentItem.AttachmentType.Other:
                    break;
                default:
                    break;
            }
            
        }
        private void SwapWeapon()
        {
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (mainWeapon == null) return;
                if (carryWeapon == mainWeapon) return;
                if (carryWeapon.gameObject.activeInHierarchy)
                {
                    StartWaitingForHolsterEnd();
                    carryWeapon.gunAnimator.SetTrigger("Holster");
                }
                else
                {
                    SetupCarriedWeapon(mainWeapon);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (secondWeapon == null) return;
                if (carryWeapon == secondWeapon) return;
                if (carryWeapon.gameObject.activeInHierarchy)
                {
                    StartWaitingForHolsterEnd();
                    carryWeapon.gunAnimator.SetTrigger("Holster");
                }
                else
                {
                    SetupCarriedWeapon(secondWeapon);
                }
            }
        }
        //启动协程
        private void StartWaitingForHolsterEnd()
        {
            if (waitingForHolsterEnd == null)
            {
                waitingForHolsterEnd = WaitingForHolsterEnd();
            }
            StartCoroutine(waitingForHolsterEnd);
        }
        //收枪协程
        private IEnumerator WaitingForHolsterEnd()
        {
            while (true)
            {
                AnimatorStateInfo tmp_AnimatorStateInfo = carryWeapon.gunAnimator.GetCurrentAnimatorStateInfo(0);
                if (tmp_AnimatorStateInfo.IsTag("Holster"))
                {
                    if (tmp_AnimatorStateInfo.normalizedTime >= 0.9f)
                    {
                        //动画播放完后才切换武器
                        Firearms tmp_TargetWeapon = carryWeapon == mainWeapon ? secondWeapon : mainWeapon;
                        SetupCarriedWeapon(tmp_TargetWeapon);

                        waitingForHolsterEnd = null;
                        yield break;
                    }
                }
                yield return null;
            }
        }
        private void SetupCarriedWeapon(Firearms _targetWeapon)
        {
            if (carryWeapon)
            {
                carryWeapon.gameObject.SetActive(false);
            }
            carryWeapon = _targetWeapon;
            carryWeapon.gameObject.SetActive(true);
            characterController.SetupAnimator(_targetWeapon.gunAnimator);
        }
    }
}