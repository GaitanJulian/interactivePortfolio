using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 500f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float strafeSpeed = 300f;
    [SerializeField] private float jumpForce = 1500f;
    [SerializeField] private Rigidbody hips;

    private PlayerControls controls;
    public bool isGrounded;
    private bool isSprinting;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Enable();
        // controls.Player.Move.performed += Move;
        controls.Player.Sprint.performed += ToggleSprint;
        controls.Player.Jump.performed += Jump;
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = controls.Player.Move.ReadValue<Vector2>();
        // Move forward and backward

        Vector3 movement = transform.forward * inputVector.y * speed;

        // Strafe left and right
        movement += transform.right * inputVector.x * strafeSpeed;

        // Apply sprinting speed if sprinting
        if (isSprinting)
        {
            movement *= sprintSpeedMultiplier;
        }

        // Apply movement force
        hips.AddForce(movement);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            hips.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        isSprinting = !isSprinting;
    }

}
