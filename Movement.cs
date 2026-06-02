
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public int maxJumps = 2;

    [Header("Better Jump")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.15f;

    private Rigidbody2D rb;

    private float moveInput;

    private int jumpsLeft;

    private float coyoteCounter;
    private float jumpBufferCounter;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        if (isGrounded)
        {
            jumpsLeft = maxJumps;
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0)
        {
            if (coyoteCounter > 0 || jumpsLeft > 1)
            {
                Jump();

                jumpBufferCounter = 0;

                if (coyoteCounter <= 0)
                    jumpsLeft--;
            }
        }

        BetterJump();

        Flip();
    }

    void FixedUpdate()
    {
        rb.linearVelocity =
            new Vector2(moveInput * moveSpeed,
                        rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity =
            new Vector2(rb.linearVelocity.x, 0);

        rb.AddForce(
            Vector2.up * jumpForce,
            ForceMode2D.Impulse
        );
    }

    void BetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity +=
                Vector2.up *
                Physics2D.gravity.y *
                (fallMultiplier - 1) *
                Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 &&
                 !Input.GetButton("Jump"))
        {
            rb.linearVelocity +=
                Vector2.up *
                Physics2D.gravity.y *
                (lowJumpMultiplier - 1) *
                Time.deltaTime;
        }
    }

    void Flip()
    {
        if (moveInput > 0)
        {
            transform.localScale =
                new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale =
                new Vector3(-1, 1, 1);
        }
    }
}
