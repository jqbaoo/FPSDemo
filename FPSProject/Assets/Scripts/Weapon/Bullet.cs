using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    public class Bullet : MonoBehaviour
    {
        public float bulletSpeed;
        public GameObject impactPrefab;
        public ImpactAudioData impactAudioData;
        /// <summary>
        /// 伤害值
        /// </summary>
        public float damageValue;

        //由于子弹过多会造成性能问题，所以此处不用rigidbody控制子弹的移动
        //直接修改Transform
        //private Rigidbody bulletRigidbody;

        private Transform bulletTransform;
        private Vector3 prevPosition;
        void OnEnable()
        {
            StartCoroutine(InPool());
        }
        void Start()
        {
            //bulletRigidbody = transform.GetComponent<Rigidbody>();

            bulletTransform = transform;
            prevPosition = bulletTransform.position;

            //impactPrefab = Resources.Load<GameObject>("Prefabs/Bullet_GoldFire_Medium_Impact");
            
        }
        private ParticleSystem impactEffect;
        void Update()
        {
            prevPosition = bulletTransform.position;
            bulletTransform.Translate(0, 0, bulletSpeed * Time.deltaTime);

            RaycastHit tmp_Hit;
            if (Physics.Raycast(prevPosition, (bulletTransform.position - prevPosition).normalized, out tmp_Hit, (bulletTransform.position - prevPosition).magnitude))
            {
                GameObject tmp_BulletEffect = Instantiate(impactPrefab, tmp_Hit.point, Quaternion.LookRotation(tmp_Hit.normal, Vector3.up));
                Destroy(tmp_BulletEffect, 3f);

                //impactEffect = GameObjectPool.Instance.OutPool(impactPrefab, tmp_Hit.point, Quaternion.LookRotation(tmp_Hit.normal, Vector3.up)).GetComponent<ParticleSystem>();
                

                foreach (ImpactTagWithAudio tmp_ImpactClip in impactAudioData.impactTagWithAudio)
                {
                    if (tmp_Hit.collider.CompareTag(tmp_ImpactClip.tag))
                    {
                        int tmp_ImpactAudioClipsCount = tmp_ImpactClip.impactAudioClips.Count;
                        AudioClip tmp_AudioClicp = tmp_ImpactClip.impactAudioClips[Random.Range(0, tmp_ImpactAudioClipsCount)];
                        AudioSource.PlayClipAtPoint(tmp_AudioClicp, tmp_Hit.point, 1);
                    }
                }
            }
        }
        private IEnumerator InPool()
        {
            yield return new WaitForSeconds(1f);
            GameObjectPool.Instance.InPool(this.gameObject);
        }
    }
}