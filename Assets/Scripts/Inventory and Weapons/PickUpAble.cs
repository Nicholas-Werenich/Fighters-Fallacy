
using UnityEngine;
using UnityEngine.UI;

public class PickUpAble : MonoBehaviour
{
    public string weaponType;

    private Sprite openChest;
    private Inventory inventory;
    private GameObject canvas;
    private bool playerInbounds;
    private bool isChest = false;

    private void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
        canvas = transform.GetChild(0).gameObject;
        
        if(gameObject.tag != "WeaponPickup")
        {
            isChest = true;
            openChest = Resources.Load<Sprite>("Sprites/Chest Open");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInbounds)
            OnPickup();
    }

    private void OnPickup()
    {
        inventory.AddWeapon(weaponType);
        canvas.SetActive(false);

        if (isChest)
        {
            GetComponent<SpriteRenderer>().sprite = openChest;
            Destroy(GetComponent<PickUpAble>());
        }
        else
            Destroy(gameObject);
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
