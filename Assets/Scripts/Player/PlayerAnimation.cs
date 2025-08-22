using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float attackCoolDown;
    private float currentAttackCoolDown;

    private PlayerMovement playerMovement;
    private Inventory inventory;
    private GameObject weaponPos;
    private Animator animator;
    private Rigidbody2D rb;
    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        weaponPos = GameObject.Find("WeaponPosition");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        inventory = GetComponent<Inventory>();
    }
    void Update()
    {
       
        if (Time.timeScale == 0)
            return;

        currentAttackCoolDown += Time.deltaTime;

        Running();
        
        Jumping();
        Attacking();
        InAir();

        if (Input.GetKeyDown(KeyCode.A))
            Flip("left");
        else if (Input.GetKeyDown(KeyCode.D))
            Flip("right");
    }

    private void Running()
    {
        if (Mathf.Abs(rb.linearVelocityX) > 0.1f)
            animator.SetBool("IsRunning", true);
        else
            animator.SetBool("IsRunning", false);

        animator.SetFloat("Speed", Mathf.InverseLerp(0.1f, 5,Mathf.Abs(rb.linearVelocityX)) * 3);
    }
    public void Flip(string direction)
    {
        switch (direction.ToLower())
        {
            case "left":
                transform.rotation = new Quaternion(0, 180, 0, 0);
                if (!ReferenceEquals(inventory.weapons[inventory.activeSlot], null))
                    weaponPos.GetComponentInChildren<SpriteRenderer>().sortingOrder = -1;
                break;
            case "right":
                transform.rotation = new Quaternion(0, 0, 0, 0);
                if (!ReferenceEquals(inventory.weapons[inventory.activeSlot], null))
                    weaponPos.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
                break;
        }
    }

    private void InAir()
    {
        if (playerMovement.IsGrounded())
            animator.SetBool("InAir", false);
        else
            animator.SetBool("InAir", true);
    }
    private void Jumping()
    {
        if (playerMovement.IsGrounded() && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)))
            animator.SetTrigger("Jumping");
    }


    private void Attacking()
    {
        if (Input.GetMouseButtonDown(0) && currentAttackCoolDown >= attackCoolDown)
        {
            currentAttackCoolDown = 0;
            //inventory.weapons[inventory.activeSlot].name == "Bow"
            animator.SetTrigger("Attacking");
        }

    }
}
