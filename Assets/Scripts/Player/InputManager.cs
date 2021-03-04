using UnityEngine;

public class InputManager : MonoBehaviour
{
    /*
     * With the input manager, things like movement are handled by modifying variables that are then read in other classes. 
     * When a button is pressed though, we use methods to return booleans. Some buttons require up and down notification,
     * while others like jumping only require a trigger notification.
    */

    public static InputManager instance { get; private set;}

    private PlayerControls controls;

    public Vector2 move { get; private set; }
    public Vector2 look { get; private set; }

    public bool fireDown { get; private set; } = false;
    public bool aimDown { get; private set; } = false;
    public bool jumpDown { get; private set; } = false;

    public bool reloadDown { get; private set; } = false;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        } else
        {
            instance = this;
        }

        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        //moving and looking
        controls.Locomotion.Movement.performed += controls => move = controls.ReadValue<Vector2>();
        controls.Locomotion.Look.performed += controls => look = controls.ReadValue<Vector2>();

        //left mouse down and up
        controls.Locomotion.Fire.performed += controls => fireDown = true;
        controls.Locomotion.Fire.canceled += controls => fireDown = false;

        //right mouse down and up
        controls.Locomotion.Aim.performed += controls => aimDown = true;
        controls.Locomotion.Aim.canceled += controls => aimDown = false;

        //spacebar down and up
        controls.Locomotion.Jump.performed += controls => jumpDown = true;
        controls.Locomotion.Jump.canceled += controls => jumpDown = false;

        controls.Locomotion.Reload.performed += controls => reloadDown = true;
        controls.Locomotion.Reload.canceled += controls => reloadDown = false;
    }

}
