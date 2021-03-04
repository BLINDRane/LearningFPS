using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //SETUP VARS
    [Header("Setup")]
    public Camera mainCam;
    public Camera scopeCam;
    public Transform muzzle;
    private InputManager inputManager; //TODO: Consider referencing all inputs from the player controller, or rather having the player controller notify whatever gun it is holding when to shoot.

    //COMBAT STAT VARS
    [Header("Combat Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float adsSpeed = 8f;
    public float rpm = 100; 
    private float fireRate; //fire rate in seconds
    private float nextTimeToFire = 0f;

    //FX VARS
    [Header("FX Variables")]
    public ParticleSystem muzzleFlash;
    public GameObject bulletTrail;
    public GameObject bloodSplat;
    private AudioSource audioSource;

    //ADS VARS
    [Header("ADS Positioning")]
    public Vector3 originalPosition;
    public Vector3 aimPosition;
    public GameObject hipReticle;

    //RECOIL STAT VARS
    [Header("Recoil Stats")]
    public float hipBloom;
    public float vRecoil;
    public float hRecoil;
    public float kickback;
    private Quaternion originalRotation;

    private void Start()
    {
        inputManager = InputManager.instance;
        audioSource = GetComponent<AudioSource>();

        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        fireRate = 60 / rpm;
    }

    void Update()
    {
        if (inputManager.fireDown && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        } else if(!inputManager.aimDown)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 4f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * 4f);
        } else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * 4f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * 4f);
        }
  
        AimDownSights();
        
    }

    private void Shoot()
    {
        if (inputManager.aimDown)
        {
            ADSShoot();
        } else
        {
            HipShoot();
        }

    }

    private void ADSShoot()
    {
        //fx
        audioSource.Play();
        muzzleFlash.Play();
        Instantiate(bulletTrail, muzzle.position, muzzle.rotation, muzzle);

        //actual shot
        RaycastHit hit;

        if (Physics.Raycast(scopeCam.transform.position, scopeCam.transform.forward, out hit, range))
        {
            Debug.Log("hit " + hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();

            if (target != null)
            {
                target.TakeDamage(damage);
                var temp = Instantiate(bloodSplat, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(temp, 2);
            }
        }

        //RECOIL
        transform.Rotate(-vRecoil, hRecoil, 0);
        transform.position -= transform.forward * kickback;
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
            Debug.Log("hit " + hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();

            if (target != null)
            {
                target.TakeDamage(damage);
                var temp = Instantiate(bloodSplat, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(temp, 2);
            }
        }

        //RECOIL
        transform.Rotate(-vRecoil, hRecoil, 0);
        transform.position -= transform.forward * kickback;
    }

    private void AimDownSights()
    {
        if (inputManager.aimDown) //can you aim and reload at the same time?
        {
            if (hipReticle.activeSelf)
            {
                hipReticle.SetActive(false);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * adsSpeed);
        } else
        {
            if (!hipReticle.activeSelf)
            {
                hipReticle.SetActive(true);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * adsSpeed);
        }
    }
}
