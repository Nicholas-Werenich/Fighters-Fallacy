using UnityEngine;

public class Bobbing : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobHeight;
    public float bobSpeed;

    private float startY;
    private bool hasLanded = false;

    public LayerMask ground;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (!hasLanded)
        {

            if (Physics2D.Raycast(transform.position, Vector2.down, bobHeight + 0.5f, ground))
            {
                hasLanded = true;
                startY = transform.position.y;
                rb.gravityScale = 0;
            }
        }
        else
        {
            float newY = startY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}