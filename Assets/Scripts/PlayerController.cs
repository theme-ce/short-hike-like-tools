using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6.0f;
    public float rotationSpeed = 10.0f;
    public Camera playerCamera;

    private CharacterController controller;
    private Vector3 velocity;

    private float gravity = -9.81f;
    private float groundedGravity = -0.05f;
    private bool isJumpPressed = false;
    private float initialJumpVelocity;
    private float maxJumpHeight = 1f;
    private float maxJumpTime = 0.5f;
    private bool isJumping = false;

    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform groundCheck;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        SetupJumpVariables();
    }

    private void SetupJumpVariables()
    {
        float timeToApex = maxJumpHeight / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveInput = new Vector3(x, 0, z);
        Vector3 moveDirection = playerCamera.transform.TransformDirection(moveInput);
        moveDirection.y = 0;
        moveDirection.Normalize();

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        HandleGravity();
        HandleJump();

        controller.Move(velocity * Time.deltaTime);

        // Rotate the character based on the camera's rotation
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            isJumpPressed = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumpPressed = false;
        }

        if (!isJumping && isGrounded && isJumpPressed)
        {
            isJumping = true;
            velocity.y = initialJumpVelocity;
        }
        else if (!isJumpPressed && isJumping && isGrounded)
        {
            isJumping = false;
        }
    }

    private void HandleGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            velocity.y = groundedGravity;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }
}
