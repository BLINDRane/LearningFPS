using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController controller;
    private InputManager inputManager;


    [SerializeField]
    private float runSpeed = 5;
    [SerializeField]
    private float walkSpeed = 3;
    [SerializeField]
    private float sprintSpeed = 7.5f;
    [SerializeField]
    private float jumpHeight = 3f;

    #region Handling Gravity
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private bool grounded;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float groundDistance = .4f;
    #endregion

    void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.instance;
    }

    void FixedUpdate()
    {
        HandleMovement(Time.deltaTime);
        HandleGravity(Time.deltaTime);
        HandleJump(Time.deltaTime);
    }

    private void HandleMovement(float delta)
    {
        //multiplying by transform.right and transform.forward will make all movement relative to camera direction. 
        Vector3 movement = (inputManager.move.x * transform.right) + (inputManager.move.y * transform.forward);
        
        controller.Move(movement * runSpeed * delta);
    }

    private void HandleGravity(float delta)
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * delta;
        controller.Move(velocity * delta);
    }

    private void HandleJump(float delta)
    {
        if (inputManager.jumpDown && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
