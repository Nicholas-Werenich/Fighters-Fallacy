using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    public GameObject healthPeiceFull;
    public GameObject healthPeiceEmpty;
    private GameObject canvas;
    private Animator animator;

    public float healthHeight;
    public float spaceBetweenHealthChunks;
    public int totalHealth;

    public ParticleSystem deathParticles;
    [HideInInspector]
    public int currentHealth;

    public float deathTime;

    private float canvasWidth;
    private List<GameObject> fullHealthChunks = new List<GameObject>();

    private float animationLength;
    private bool death = false;
    
    private Vector3 targetPosition;
    private Vector2 velocity = Vector2.zero;
    private float healthSize;

    // Start is called before the first frame update
    void Awake()
    {
        canvas = transform.GetChild(0).gameObject;
        //healthSize = healthPeiceFull.GetComponent<RectTransform>().sizeDelta[0];
        animator = GetComponent<Animator>();
        healthUI(healthPeiceEmpty);
        healthUI(healthPeiceFull, true);
        currentHealth = totalHealth;
    }

    private void healthUI(GameObject piece, bool fullHealth = false)
    {
        //double this for actual health and then add to list
        
        for (int i = 0; i < totalHealth; i++)
        {
            GameObject currentPiece = Instantiate(piece, Vector3.zero, Quaternion.identity);
            RectTransform healthWidth = currentPiece.AddComponent<RectTransform>();
            healthWidth.SetParent(canvas.transform);
            healthWidth.localScale = Vector3.one;
            healthWidth.sizeDelta = new Vector2(healthHeight, healthHeight);
            healthWidth.localPosition = new Vector3(i * spaceBetweenHealthChunks + i * (healthWidth.sizeDelta[0] * 2) - ((totalHealth - 1) * (healthWidth.sizeDelta[0] * 2) + spaceBetweenHealthChunks * (totalHealth - 1)) / 2, 0, 0);
            //healthWidth.localPosition = new Vector3(i * spaceBetweenHealthChunks + (i * (canvasWidth / totalHealth)) - canvasWidth / 2 + width / 2, 0, 0);
            if (fullHealth == true) fullHealthChunks.Add(currentPiece);
        }
    }

    public void takeDamage(int damage = 1)
    {
        //insert damage animation
        for(int i = currentHealth -1;  i > currentHealth-damage-1; i--)
        {
            if (i <= 0)
                StartCoroutine(Death());
            else
                fullHealthChunks[i].SetActive(false);
        }
        if(currentHealth > 0)
            animator.SetTrigger("Hit");
        currentHealth -= damage;
        
        //access last health point through list and remove
    }

    private IEnumerator Death()
    {
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("Dead");
        try
        {
            GetComponent<EnemyAI>().enabled = false;
        }
        catch { }


        foreach (AnimationClip animation in animator.runtimeAnimatorController.animationClips)
        {
            if("EnemyDead" == animation.name)
            {
                
                transform.GetChild(0).gameObject.SetActive(false);

                yield return new WaitForSeconds(animation.length);
                death = true;
                yield return new WaitForSeconds(deathTime * 1.5f);
                break;
            }
        }

        Instantiate(deathParticles, transform.position, transform.rotation);
        deathParticles.Play();

        Destroy(gameObject);
    }


    private void Update()
    {
        if (death)
        {
            transform.localScale = Vector2.SmoothDamp(transform.localScale, new Vector2(2,2), ref velocity, deathTime);
        }
    }


    public void heal(int heal = 1)
    {
        if (currentHealth != totalHealth)
        {
            currentHealth++;
            fullHealthChunks[currentHealth - 1].SetActive(true);
        }

        //Add another health to list and put it a spaceBetween after the last one
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            int damage = other.gameObject.GetComponent<Attack>().damage;
            other.gameObject.GetComponent<Attack>().durability--;
            takeDamage(damage);
        }
    }
}
