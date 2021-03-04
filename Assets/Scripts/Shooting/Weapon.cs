using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected InputManager input;

    [Header("Components")]
    public Camera mainCam;
    public Transform muzzle;
    public Transform sight;
    public Transform parent;

    [Header("Combat Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float adsSpeed = 8f;
    public int magSize = 30;
    public int curAmmo = 30;
    public int maxAmmo = 210;

    [Header("FX Variables")]
    public ParticleSystem muzzleFlash;
    public GameObject bulletTrail;
    public GameObject bloodSplat;
    protected AudioSource audioSource;

    [Header("ADS Positioning")]
    public Vector3 originalPosition;
    public Vector3 aimPosition;
    public GameObject hipReticle;
    public bool canADS = true;

    [Header("Recoil Stats")]
    public float hipBloom;
    public float vRecoil;
    public float hRecoil;
    public float kickback;
    protected Quaternion originalRotation;

    [Header("Operational Info")]
    public bool canFire;
    public bool reloading;

    // Start is called before the first frame update
    protected void Init()
    {
        input = InputManager.instance;
        audioSource = GetComponent<AudioSource>();

        originalPosition = parent.localPosition;
        originalRotation = parent.localRotation;

        curAmmo = magSize;
    }

    protected abstract void CheckForFire();

    protected void Shoot()
    {
        if (input.aimDown)
        {
            ADSShoot();
        }
        else
        {
            HipShoot();
        }

        curAmmo--;
    }

    private void ADSShoot()
    {
        //fx
        audioSource.Play();
        muzzleFlash.Play();
        Instantiate(bulletTrail, muzzle.position, muzzle.rotation, muzzle);

        //actual shot
        RaycastHit hit;

        if (Physics.Raycast(sight.position, sight.forward, out hit, range))
        {

            Target target = hit.transform.GetComponent<Target>();

            if (target != null)
            {
                target.TakeDamage(damage);

                //hit confirmation
                ConfirmHit();

                var temp = Instantiate(bloodSplat, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(temp, 2);
            }
        }

        //RECOIL
        parent.Rotate(-vRecoil, hRecoil, 0);
        parent.position -= transform.forward * kickback;
    }

    private void HipShoot()
    {
        //bloom calculation
        Vector3 calcBloom = mainCam.transform.position + mainCam.transform.forward * range;
        calcBloom += Random.Range(-hipBloom, hipBloom) * mainCam.transform.up;
        calcBloom += Random.Range(-hipBloom, hipBloom) * mainCam.transform.right;
        calcBloom -= mainCam.transform.position;
        calcBloom.Normalize();

        //fx
        audioSource.Play();
        muzzleFlash.Play();
        Instantiate(bulletTrail, muzzle.position, Quaternion.LookRotation(calcBloom));

        //actual shot
        RaycastHit hit;

        if (Physics.Raycast(mainCam.transform.position, calcBloom, out hit, range))
        {

            Target target = hit.transform.GetComponent<Target>();

            if (target != null)
            {
                target.TakeDamage(damage);

                //hit confirmation
                ConfirmHit();

                var temp = Instantiate(bloodSplat, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(temp, 2);
            }
        }

        //RECOIL
        parent.Rotate(-vRecoil, hRecoil, 0);
        parent.position -= parent.transform.forward * kickback;
    }

    protected void AimDownSights()
    {
        if (input.aimDown) //can you aim and reload at the same time?
        {
            if (hipReticle.activeSelf)
            {
                hipReticle.SetActive(false);
            }

            parent.localPosition = Vector3.Lerp(parent.localPosition, aimPosition, Time.deltaTime * adsSpeed);
        }
        else
        {
            if (!hipReticle.activeSelf)
            {
                hipReticle.SetActive(true);
            }

            parent.localPosition = Vector3.Lerp(parent.localPosition, originalPosition, Time.deltaTime * adsSpeed);
        }
    }

    protected void CheckForReload()
    {
        if (input.reloadDown && curAmmo < magSize && !reloading)
        {
            reloading = true;
            //play reload anim
            StartCoroutine(ReloadNoAnim());
        }
    }

    public void CompleteReload()
    {
        curAmmo += magSize - curAmmo;
        reloading = false;
    }

    protected void ResetPosition()
    {
        if (!input.aimDown)
        {
            parent.localPosition = Vector3.Lerp(parent.localPosition, originalPosition, Time.deltaTime * 4f);
            parent.localRotation = Quaternion.Lerp(parent.localRotation, originalRotation, Time.deltaTime * 4f);
        }
        else
        {
            parent.localPosition = Vector3.Lerp(parent.localPosition, aimPosition, Time.deltaTime * 4f);
            parent.localRotation = Quaternion.Lerp(parent.localRotation, originalRotation, Time.deltaTime * 4f);
        }
    }

    protected void ConfirmHit()
    {
        //hitMarker.SetActive(true);
    }

    protected IEnumerator ReloadNoAnim()
    {
        yield return new WaitForSecondsRealtime(4);
        CompleteReload();
    }
}
