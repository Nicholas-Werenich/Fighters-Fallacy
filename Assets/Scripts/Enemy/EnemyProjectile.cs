using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float resetTime;
    public ParticleSystem explodeParticles;
    private float lifetime;


    private void Awake()
    {
        lifetime = 0;
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            Explode();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Weapon" || collision.gameObject.layer == 3)
            StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForEndOfFrame();
        ParticleSystem explode = Instantiate(explodeParticles, transform.position, Quaternion.identity);
        explode.Play();

        Destroy(gameObject);
    }

}