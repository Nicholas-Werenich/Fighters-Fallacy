using UnityEngine;

public class PlayerProjectile : Attack
{
    [Header("Projectile Settings")]
    public GameObject pickup;
    public string weaponType;

    public float liftime;
    private float lifeDuration;

    private void Awake()
    {
        lifeDuration = 0;
    }
    private void Update()
    {
        lifeDuration += Time.deltaTime;
        if (lifeDuration > liftime)
            End();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Enemy" || collision.gameObject.layer == 3)
        {
            Debug.Log(collision.name);
            End();
        }
            
    }

    private void End()
    {
        GameObject arrow = Instantiate(pickup, transform.position, Quaternion.identity);
        transform.localRotation = Quaternion.identity;
        arrow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        arrow.GetComponent<PickUpAble>().weaponType = weaponType;
        Destroy(gameObject);
    }
}
