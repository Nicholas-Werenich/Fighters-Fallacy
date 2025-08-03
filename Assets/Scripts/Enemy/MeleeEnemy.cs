using System.Collections;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float spottedSpeed;
    [SerializeField] private float range;
    [SerializeField] private int damage;


    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    //References
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyPatrol enemyPatrol;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        //Attack only when player in sight?
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("Attacking");
            }
        }
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }
    


    private bool PlayerInSight()
    {
        return Physics2D.BoxCast(transform.position, transform.localScale, 0, -transform.right, range, playerLayer);
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
    */
    private void FixedUpdate()
    {
        if (Physics2D.Raycast(transform.position, -transform.right, range, playerLayer) && transform.position.x >= enemyPatrol.leftEdge.position.x && transform.position.x <= enemyPatrol.rightEdge.position.x)
        {
            rb.AddForce(-transform.right * spottedSpeed);
            anim.SetFloat("Speed", 2f);
        }
        else
        {
            enemyPatrol.usedSpeed = enemyPatrol.speed;
            anim.SetFloat("Speed", 1f);
        }
    }
}