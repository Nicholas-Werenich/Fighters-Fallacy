using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]

    public Transform target;
    public float activateDistance;
    public float pathUpdateSeconds = 0.5f;


    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeight = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;
    private Rigidbody2D targetrb;
    public LayerMask ground;
    private CircleCollider2D col;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public float attackDistance;
    public float attackSpeed;
    private float attackSpeedTimer;

    private Path path;
    private int currentWaypoint = 0;
    private bool isGrounded = false;

    [Header("Animation")]
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool inAir;

    Seeker seeker;

    Rigidbody2D rb;
    private void Awake()
    {
        col = GetComponent<CircleCollider2D>(); 
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        target = GameObject.Find("Player").transform;
        targetrb = target.GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (AttackDistance())
        {
            Attack();
            animator.SetBool("IsMoving", false);
        }
        else if (TargetInDistance())
        {
            PathFollow();
        }
        else
            animator.SetBool("IsMoving", false);

    }

    private void Attack()
    {
        if(attackSpeedTimer < 0f)
        {
            animator.SetTrigger("Attacking");
            attackSpeedTimer = attackSpeed;
        }
        attackSpeedTimer -= Time.deltaTime;
    }

    private void UpdatePath()
    {
        if(followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
            return;

        //Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y, + jumpCheckOffset);
        isGrounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.1f, ground);

        Vector2 direction = (Vector2)path.vectorPath[currentWaypoint] - rb.position;
        Vector2 force = direction * speed * Time.deltaTime;

        if( isGrounded)
        {
            if(direction.y > jumpNodeHeight)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
                animator.SetTrigger("Jumping");
                Debug.Log("Jump needed");
            }

            if (inAir)
            {
                animator.SetTrigger("Landing");
                inAir = false;
            }
                

        }
        else
            inAir = true;





        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (rb.linearVelocityX > 0f)
            transform.rotation = new Quaternion(0, 180, 0, 0);
        else if (rb.linearVelocityX < 0f)
            transform.rotation = new Quaternion(0, 0, 0, 0);

        animator.SetBool("IsMoving", true);
    }


    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private bool AttackDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < attackDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }

    }

}
