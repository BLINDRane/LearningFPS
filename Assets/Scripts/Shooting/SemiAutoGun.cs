using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutoGun : Weapon
{
    // Start is called before the first frame update
    void Start()
    {
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForFire();
        AimDownSights();
        ResetPosition();
        CheckForReload();
    }

    protected override void CheckForFire()
    {
        if (!input.fireDown && curMag > 0 && !reloading)
        {
            canFire = true;
        }

        if (reloading)
        {
            canFire = false;
        }

        if (input.fireDown && canFire)
        {
            Shoot();
            canFire = false;
        }


    }
}
