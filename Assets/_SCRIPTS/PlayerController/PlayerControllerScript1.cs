using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControllerScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isGrounded;

    public Transform groundCheck;
    public LayerMask groundLayer;

    public float groundCheckRadius = 0.3f;
    public Vector3 groundCheckOffset = new Vector3(0, -0.5f, 0); // Posici�n relativa al jugador

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck no asignado, creando por defecto.");
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = groundCheckOffset;
            groundCheck = gc.transform;
        }
    }

    void Update()
    {
        HandleMovement(Input.GetAxis("Horizontal"));

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    public void FixedUpdate()
    {
        if (groundCheck == null)
            return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius, isGrounded ? Color.green : Color.red);
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
