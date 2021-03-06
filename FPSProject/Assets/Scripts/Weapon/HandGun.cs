﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    public class HandGun:Firearms
    {
        private FPMouseLook fpMouseLook;
        protected override void Start()
        {
            base.Start();
            fpMouseLook = FindObjectOfType<FPMouseLook>();
        }
        
        protected override void Shooting()
        {
            if (currentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            muzzleParticle.Play();
            currentAmmo -= 1;

            gunAnimator.Play("Fire", isAim ? 1 : 0, 0);

            shootingAudioSource.clip = firearmsAudioData.shootingAudio;
            shootingAudioSource.Play();

            CreateBullet();
            casingParticle.Play();
            lastFireTime = Time.time;
            
            //枪口抖动
            fpMouseLook.CollimationOffset();
        }
        protected override void Reload()
        {
            gunAnimator.SetLayerWeight(2, 1);
            gunAnimator.SetTrigger(currentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");

            reloadAudioSource.clip = currentAmmo > 0 ? firearmsAudioData.reloadLeftAudio : firearmsAudioData.reloadOutOfAudio;
            reloadAudioSource.Play();

            if (ReloadAmmoCheckCoroutine == null)
            {
                ReloadAmmoCheckCoroutine = CheckReloadAmmoAnimatorEnd();
                StartCoroutine(ReloadAmmoCheckCoroutine);
            }
            else
            {
                StartCoroutine(ReloadAmmoCheckCoroutine);
                ReloadAmmoCheckCoroutine = null;
                ReloadAmmoCheckCoroutine = CheckReloadAmmoAnimatorEnd();
                StartCoroutine(ReloadAmmoCheckCoroutine);
            }
        }
        protected override Bullet CreateBullet()
        {
            Bullet tmp_BulletScript = base.CreateBullet();
            tmp_BulletScript.bulletSpeed = 500f;
            tmp_BulletScript.damageValue = 15;

            return tmp_BulletScript;
        }
    }
}