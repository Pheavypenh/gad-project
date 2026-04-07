using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius = 0.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError("Missing CharacterController on Player!");

        Debug.Log("PlayerMovement started - Ground Layer: " + LayerMask.LayerToName(groundLayer));
    }

    void Update()
    {
        // Use both checks for reliability
        bool sphereGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);
        isGrounded = sphereGrounded || controller.isGrounded;

        // Reset velocity when grounded
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Move in the direction the player is facing
        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move.normalized * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("=== JUMP TRIGGERED! ===");
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Debug
        if (Time.frameCount % 30 == 0)  // every 30 frames
            Debug.Log($"Grounded: {isGrounded} (Sphere: {sphereGrounded}, CC: {controller.isGrounded}) | Input: H={h} V={v}");
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}