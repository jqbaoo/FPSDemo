using Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    private Text txt_CurrentAmmo;
    private Button btn_Reset;
    public WeaponManager weaponManager;
    void Awake()
    {
        txt_CurrentAmmo = transform.Find("Txt_Ammo").GetComponent<Text>();
        btn_Reset = transform.Find("Btn_Reset").GetComponent<Button>();
        btn_Reset.onClick.AddListener(OnReset);
    }
    public void UpdateUI(int _currentAmmo,int _LeftCurrentAmmo)
    {
        txt_CurrentAmmo.text = _currentAmmo.ToString() + " / " + _LeftCurrentAmmo.ToString();
    }

    // 重置子弹
    private void OnReset()
    {
        weaponManager.carryWeapon.GetCurrentMaxAmmoCarried = weaponManager.mainWeapon.maxAmmoCarried;
    }
}
