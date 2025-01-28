using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 3f; // Movement speed
    public float jumpForce = 5f; // Force applied when jumping
    public Animator animator;
    private Rigidbody rb;

    private bool isGrounded = true; // To check if the character is on the ground

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        // Get movement inputs (ZQSD for movement)
        float moveX = -Input.GetAxisRaw("Horizontal"); // Left/Right (Q/D)
        float moveZ = -Input.GetAxisRaw("Vertical"); // Forward/Backward (Z/S)

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        // Apply movement if there is input
        if (move.magnitude > 0)
        {
            // Smooth movement using MoveTowards for smoother transitions
            Vector3 targetPosition = transform.position + move * speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Smooth rotation to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            // Trigger movement animation
            animator.SetTrigger("MovingTrigger");
            animator.SetBool("isMoving", true);
        }
        else
        {
            // Stop movement animation if no input
            animator.SetTrigger("IdleTrigger");
            animator.SetBool("isMoving", false);
        }
    }


    private void HandleJump()
    {
        // Jump with the "Tab" key
        if (Input.GetKeyDown(KeyCode.Tab) && isGrounded)
        {
            isGrounded = false; // Temporarily disable ground check
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Trigger jump animations
            animator.SetTrigger("Jump");
        }
    }

    private void Attack1()
    {
        animator.SetTrigger("Attack1");
    }

    private void Attack2()
    {
        animator.SetTrigger("Attack2");
    }

    private void SpecialAttack()
    {
        animator.SetTrigger("SpecialAttack");
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     // Check if character lands on the ground
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = true;

    //         // Transition to idle if landing
    //         animator.SetTrigger("Land");
    //     }
    // }

}

