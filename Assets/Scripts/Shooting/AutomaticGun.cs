using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticGun : Weapon
{
    //COMBAT STAT VARS
    [Header("Operational Info")]
    public float rpm = 100;
    private float fireRate; //fire rate in seconds
    private float nextTimeToFire = 0f;

    private void Start()
    {
        base.Init();
        fireRate = 60 / rpm;
    }

    void Update()
    {
        CheckForFire(); 
        AimDownSights();
        ResetPosition();
        CheckForReload();
    }

    protected override void CheckForFire()
    {
        if (Time.time >= nextTimeToFire && curAmmo > 0 && !reloading)
        {
            canFire = true;
        } else
        {
            canFire = false;
        }

        if (input.fireDown && canFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }
}
