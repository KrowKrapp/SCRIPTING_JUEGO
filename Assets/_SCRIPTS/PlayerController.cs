using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isGrounded;

    public Transform groundCheck;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement(Input.GetAxis("Horizontal"));

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    public void HandleMovement(float direction)
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        if (direction != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction), 1, 1);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public Vector2 GetVelocity()
    {
        return rb.linearVelocity;
    }

    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    public bool GetGrounded()
    {
        return isGrounded;
    }
}
