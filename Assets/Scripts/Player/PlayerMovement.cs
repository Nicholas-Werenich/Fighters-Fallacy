using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.Rigidbody2D;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    
    public float normalSpeed;
    public float sprintModifier;
    public float jumpForce;
    public float drag;
    public float airDrag;

    private float movementSpeed;

    [Header("CoyoteTime")]
    public float coyoteTime;
    private float coyoteTimeCounter;

    [Header("Ground")]
    public float groundDistance;
    public float maxSlopeAngle;
    public float fallingGravity;
    private float normalGravity;

    private BoxCollider2D col;
    private RaycastHit2D slopeHit;

    public LayerMask ground;


    private float horizontal;

    [HideInInspector]
    public Rigidbody2D rb;
    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        normalGravity = rb.gravityScale;
    }

    private void Update()
    {
        Sprint();

        if (IsGrounded() && rb.linearVelocity.y <= 0.05f)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        rb.linearDamping = IsGrounded() ? drag : airDrag;


        if (coyoteTimeCounter > 0f && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)))
            Jump();

        GetInput();
    }

    private void GetInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    private void Sprint()
    {
        movementSpeed = Input.GetKey(KeyCode.LeftShift) ? normalSpeed * sprintModifier : normalSpeed;

    }
    private void FixedUpdate()
    {
       if (IsOnSlope())
           SlopeMovement();
        else
        {
            Movement();
            FallingGravity();
        }
    }

    private void FallingGravity()
    {
        rb.gravityScale = rb.linearVelocityY < -0.1f ? fallingGravity : normalGravity;
    }
    private bool IsOnSlope()
    {
        if (slopeHit.collider == null) return false;
        float angle = Vector2.Angle(Vector2.up, slopeHit.normal);
        return angle < maxSlopeAngle && angle > 0;
    }

    private void Movement()
    {
        //SlideResults = rb.Slide(Vector2.right * horizontal * movementSpeed, Time.deltaTime, slideMovement);
        rb.AddForce(Vector2.right * horizontal * movementSpeed, ForceMode2D.Force);
    }

    private void SlopeMovement()
    {
        Vector2 slopeDirection = Vector2.Perpendicular(slopeHit.normal).normalized * -horizontal;
        rb.AddForce(slopeDirection * movementSpeed);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteTimeCounter = 0;
    }

    public bool IsGrounded()
    {
        //Add ground mask?
        slopeHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, groundDistance,ground); 
        return slopeHit.collider != null;
    }
}
