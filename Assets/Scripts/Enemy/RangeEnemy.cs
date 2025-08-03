using System;
using System.Collections;
using UnityEngine;


public class RangedEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;
    [SerializeField] private float speed;

    [Header("Ranged Attack")]
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject projectile;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    //References
    private Animator anim;
    private EnemyPatrol enemyPatrol;

    private void Awake()
    {
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
                StartCoroutine(Attack());
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }
    private IEnumerator Attack()
    {
        cooldownTimer = 0;
        anim.SetTrigger("Attacking");

        foreach (AnimationClip animation in anim.runtimeAnimatorController.animationClips)
        {
            if ("EnemyRangeAttack" == animation.name)
            {
                yield return new WaitForSeconds(animation.length+0.08f);
                break;
            }
        }
        

        GameObject projectileInstance = Instantiate(projectile, firepoint.position, Quaternion.identity);
        projectileInstance.GetComponent<Rigidbody2D>().AddForce(-transform.right * speed);
    }
    private bool PlayerInSight()
    {
        return Physics2D.BoxCast(transform.position, transform.localScale, 0, -transform.right, range, playerLayer);
    }
}