
using UnityEngine;
using UnityEngine.UI;

public class PickUpAble : MonoBehaviour
{
    public string weaponType;

    public Sprite openChest;
    private Inventory inventory;
    private GameObject canvas;
    bool playerInbounds;

    void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        canvas = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInbounds)
        {
            inventory.AddWeapon(weaponType);
            canvas.SetActive(false);

            //Weapon or chest
            if(gameObject.tag == "WeaponPickup")
                Destroy(gameObject);
            else
            {
                GetComponent<SpriteRenderer>().sprite = openChest;
                Destroy(GetComponent<PickUpAble>());
            }
                

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            playerInbounds = true;
            canvas.SetActive(true);
        }
            
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInbounds = false;
            canvas.SetActive(false);
        }
    }
}
