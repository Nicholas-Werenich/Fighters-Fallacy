using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int knockBackForce;
    public int totalHealth;
    private int currentHealth;

    Rigidbody2D rb;
    SpriteRenderer SpriteRenderer;
    Animator animator;
    BoxCollider2D playerCollider;
    PlayerAnimation playerAnimation;
    void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        currentHealth = totalHealth;
        playerCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage = 1)
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            StartCoroutine(Death());
        }
        else
            animator.SetTrigger("Hurt");
        Debug.Log(currentHealth);
    }

    private IEnumerator Death()
    {
        Time.timeScale = 0;
        animator.SetTrigger("Dead");


        float deathLength = animator.GetCurrentAnimatorStateInfo(0).length;


        float elapsedTime = 0;

        while (elapsedTime < deathLength * 8)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;

        }

        Time.timeScale = 1;

        PlayerPrefs.SetString("Current Level", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Game Over");

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy" && other.IsTouching(playerCollider))
        {
            rb.AddForce((transform.position - other.transform.position).normalized * knockBackForce, ForceMode2D.Impulse);
            TakeDamage();
            FaceEnemy(other);
        }
    }

    private void FaceEnemy(Collider2D enemy)
    {
        if (Mathf.Abs(enemy.transform.position.x) - Mathf.Abs(transform.position.x) > 0)
            playerAnimation.Flip("right");
        else 
            playerAnimation.Flip("left");

    }
}
