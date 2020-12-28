using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void DoAttack();
    void DoReload();
    void DoAim(bool _isAim);
}
