using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    public abstract class Firearms: MonoBehaviour,IWeapon
    {
        public Transform muzzlePoint;
        public Transform casingPoint;

        public ParticleSystem muzzleParticle;
        public ParticleSystem casingParticle;
        public GameObject bulletImpactPrefab;

        public GameObject bulletPrefab;

        public AudioSource reloadAudioSource;
        public AudioSource shootingAudioSource;
        public FirearmsAudioData firearmsAudioData;
        public Camera eyeCamera;
        public Camera gunCamera;
        public ImpactAudioData impactAudioData;
        
        public List<ScopeInfo> scopeInfo;
        //无瞄具的信息
        public ScopeInfo baseIronSight;

        protected ScopeInfo currentScopeInfo;

        private IEnumerator ChangeFOVCoroutine;
        protected IEnumerator ReloadAmmoCheckCoroutine;

        /// <summary>
        /// 开枪频率
        /// </summary>
        public float fireRate;

        /// <summary>
        /// 默认填装弹匣数
        /// </summary>
        public int ammoInMag = 30;
        /// <summary>
        /// 当前弹匣中的子弹
        /// </summary>
        protected int currentAmmo;

        public int GetCurrentAmmo
        {
            get { return currentAmmo; }
            set { currentAmmo = value; }
        }
        /// <summary>
        /// 默认最大携带换弹数值
        /// </summary>
        public int maxAmmoCarried = 120;
        public Animator gunAnimator;
        /// <summary>
        /// 当前最大携带弹匣数
        /// </summary>
        protected int currentMaxAmmoCarried;
        public int GetCurrentMaxAmmoCarried
        {
            get { return currentMaxAmmoCarried; }
            set { currentMaxAmmoCarried = value; }
        }
        protected bool isAim = false;


        protected float lastFireTime;
        protected AnimatorStateInfo gunStateInfo;

        protected float originEyeFOV;
        protected float originGunFOV;
        protected Transform gunCameraTransform;
        private Vector3 originalEyePosition;

        private bool isReload = false;

        protected virtual void Start()
        {
            currentAmmo = ammoInMag;
            currentMaxAmmoCarried = maxAmmoCarried;
            //gunAnimator = GetComponent<Animator>();

            ChangeFOVCoroutine = ChangeFOV();
            ReloadAmmoCheckCoroutine = CheckReloadAmmoAnimatorEnd();

            originEyeFOV = eyeCamera.fieldOfView;
            originGunFOV = gunCamera.fieldOfView;
            gunCameraTransform = gunCamera.transform;
            originalEyePosition = gunCameraTransform.localPosition;
            currentScopeInfo = baseIronSight;
        }
        public void DoAttack()
        {
            if (isReload) return;
            Shooting();
        }
        public void DoKnife()
        {
            gunAnimator.Play("Knife");
        }
        public void DoReload()
        {
            isReload = true;
            Reload();
        }
        public void DoAim(bool _isAiming)
        {
            isAim = _isAiming;
            gunAnimator.SetLayerWeight(1, 1);
            gunAnimator.SetBool("Aim", isAim);
            
            if (ChangeFOVCoroutine == null)
            {
                ChangeFOVCoroutine = ChangeFOV();
                StartCoroutine(ChangeFOVCoroutine);
            }
            else
            {
                StopCoroutine(ChangeFOVCoroutine);
                ChangeFOVCoroutine = null;
                ChangeFOVCoroutine = ChangeFOV();
                StartCoroutine(ChangeFOVCoroutine);
            }
        }
        public void ResetAim(bool _isAiming)
        {
            isAim = _isAiming;
            gunAnimator.SetLayerWeight(1, 1);
            gunAnimator.SetBool("Aim", isAim);
        }
        public void SetupCarriedScope(ScopeInfo _scopeInfo)
        {
            if (_scopeInfo == null)
            {
                currentScopeInfo = baseIronSight;
                return;
            }
            currentScopeInfo = _scopeInfo;
        }
        protected abstract void Shooting();
        protected abstract void Reload();

        protected bool IsAllowShooting()
        {
            return Time.time - lastFireTime > 1 / fireRate;
        }
        /// <summary>
        /// 基类的创建子弹
        /// </summary>
        /// <returns></returns>
        protected  virtual Bullet CreateBullet()
        {
            GameObject tmp_Bullet = GameObjectPool.Instance.OutPool(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

            //子弹中的变量默认，若不同的枪想修改属性，直接获取方法的返回值重新设置
            Bullet tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            tmp_BulletScript.bulletSpeed = 500f;
            tmp_BulletScript.impactPrefab = bulletImpactPrefab;
            tmp_BulletScript.impactAudioData = impactAudioData;
            tmp_BulletScript.damageValue = 15;
            return tmp_BulletScript;
        }
        protected IEnumerator CheckReloadAmmoAnimatorEnd()
        {
            while (true)
            {
                yield return null;
                gunStateInfo = gunAnimator.GetCurrentAnimatorStateInfo(2);
                if (gunStateInfo.IsTag("ReloadAmmo"))
                {
                    if (gunStateInfo.normalizedTime >= 0.9f)
                    {
                        //currentMaxAmmoCarried = currentMaxAmmoCarried - currentAmmo;
                        //获取需要装填的子弹数
                        int tmp_NeedReloadAmmo = ammoInMag - currentAmmo;
                        //获取剩余的备弹
                        int tmp_currentMaxAmmoCarried = currentMaxAmmoCarried - tmp_NeedReloadAmmo;
                        //判断是否够填充满弹匣
                        if (tmp_currentMaxAmmoCarried > 0)
                        {
                            currentAmmo = ammoInMag;
                            currentMaxAmmoCarried = tmp_currentMaxAmmoCarried;
                        }
                        else
                        {
                            currentAmmo += currentMaxAmmoCarried;
                            currentMaxAmmoCarried = 0;
                        }
                        isReload = false;
                        yield break;
                    }
                }

            }
        }
        protected IEnumerator ChangeFOV()
        {
            while (true)
            {
                yield return null;
                float tmp_eyeCurrentFov = 0;
                eyeCamera.fieldOfView = Mathf.SmoothDamp(eyeCamera.fieldOfView, isAim ? currentScopeInfo.eyeFov : originEyeFOV, ref tmp_eyeCurrentFov, Time.deltaTime * 2);

                float tmp_gunCurrentFov = 0;
                gunCamera.fieldOfView = Mathf.SmoothDamp(gunCamera.fieldOfView, isAim ? currentScopeInfo.gunFov : originGunFOV, ref tmp_gunCurrentFov, Time.deltaTime * 2);

                Vector3 tmp_RefPosition = Vector3.zero;
                gunCameraTransform.localPosition = Vector3.SmoothDamp(gunCameraTransform.localPosition, isAim ? currentScopeInfo.gunCameraPosition : originalEyePosition, ref tmp_RefPosition, Time.deltaTime * 2);
            
            }
        }
    }
    [System.Serializable]
    public class ScopeInfo
    {
        public string scopeName;
        public GameObject ScopeGameObject;
        public float eyeFov;
        public float gunFov;
        public Vector3 gunCameraPosition;
    }
}
