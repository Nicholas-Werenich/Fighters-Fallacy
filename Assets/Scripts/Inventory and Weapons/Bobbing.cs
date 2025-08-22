using UnityEngine;

public class Bobbing : MonoBehaviour
{
    [Header("Bobbing Settings")]

    [SerializeField]
    private float bobHeight;

    [SerializeField]
    private float bobSpeed;

    private float startY;
    private bool hasLanded = false;

    [SerializeField]
    private LayerMask ground;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //Create a bobbing effect over ground
    private void Update()
    {
        if (!hasLanded && Physics2D.Raycast(transform.position, Vector2.down, bobHeight + 0.05f, ground))
        {
            hasLanded = true;
            startY = transform.position.y;
            rb.gravityScale = 0;
        }
        else
        {
            float newY = startY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}