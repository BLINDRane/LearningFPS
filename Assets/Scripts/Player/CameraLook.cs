using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private InputManager inputManager;

    [SerializeField]
    private Transform playerParent;

    [SerializeField]
    private float hSensitivity = 100f;
    [SerializeField]
    private float vSensitivity = 100f;
    [SerializeField]
    private float sensitivity = 100f;

    private float xRotation = 0f;

    private void Start()
    {
        inputManager = InputManager.instance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleLook(Time.deltaTime);
    }

    private void HandleLook(float delta)
    {

        float mouseX;
        float mouseY;

        //this should allow split sensitivity down the line
        if (hSensitivity == vSensitivity)
        {
            mouseX = inputManager.look.x * sensitivity * delta;
            mouseY = inputManager.look.y * sensitivity * delta;
        } else
        {
            mouseX = inputManager.look.x * vSensitivity * delta;
            mouseY = inputManager.look.y * hSensitivity * delta;
        }

        //only tilt the camera when looking up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //rotate the whole player body when looking side to side
        playerParent.Rotate(Vector3.up, mouseX);
    }
}
