using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class Inventory : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField]
    private GameObject slot;
    [SerializeField]
    private Sprite nonActiveSlotIcon;
    [SerializeField]
    private Sprite activeSlotIcon;

    private GameObject[] slots;
    private GameObject[] slotIcons;
    private GameObject[] weaponInstances;

    [Header("Weapons")]
    [SerializeField]
    private float weaponColliderSize;
    [SerializeField]
    private Transform weaponHolder;

    [HideInInspector]
    public Weapon[] weapons;

    private WeaponFactory weaponFactory;

    [Header("Slots")]
    [SerializeField]
    private int slotsAmount;
    [SerializeField]
    private float spaceBetweenSlots;
    [SerializeField]
    private float slotsSize;
    [SerializeField]
    private float hotbarHeight;

    [HideInInspector]
    public int activeSlot;

    [Header("Dropped Weapon")]
    [SerializeField]
    private GameObject pickUpTemplate;
    [SerializeField]
    private float pickUpSize;
    [SerializeField]
    private float dropForce;

    [HideInInspector]
    public bool attacking;

    private GameObject canvas;

    //Keycodes for inventory slots
    private KeyCode[] keyCodes = new KeyCode[]
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0,
    };
    private void Awake()
    {
        weaponFactory = FindFirstObjectByType<WeaponFactory>();
        slots = new GameObject[slotsAmount];
        weapons = new Weapon[slotsAmount];
        slotIcons = new GameObject[slotsAmount];
        weaponInstances = new GameObject[slotsAmount];

        canvas = transform.GetChild(0).gameObject;
        
        AddSlots();
        ActiveSlot(0);

        //Add inital weapons
    }

    //Initialize the slots in the hotbar
    public void AddSlots()
    {
        for (int i = 0; i < slotsAmount; i++)
        {
            GameObject currentPiece = Instantiate(slot, Vector3.zero, Quaternion.identity);
            currentPiece.GetComponent<Image>().sprite = nonActiveSlotIcon;
            RectTransform pieceParent = currentPiece.GetComponent<RectTransform>();
            pieceParent.SetParent(canvas.transform);
            pieceParent.localScale = Vector3.one;
            pieceParent.sizeDelta = new Vector2(slotsSize, slotsSize);
            pieceParent.localPosition = new Vector3(i * spaceBetweenSlots + i * slotsSize - ((slotsAmount - 1) * slotsSize + spaceBetweenSlots * (slotsAmount - 1)) / 2, -Screen.height / 2 + slotsSize / 2 + hotbarHeight, 0);
            slots[i] = currentPiece;
        }
    }

    public void Update()
    {
        if (!attacking)
        {
            AttackInput();
            InventoryInput();
            DropInput();
        }
    }
    
    //Switch active inventory slot
    private void InventoryInput()
    {
        for (int i = 0; i < slotsAmount; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                ActiveSlot(i);
            }
        }

    }

    //Drop the weapon in the active slot
    private void DropInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropWeapon(activeSlot);
        }
    }

    //Attack with the weapon in the active slot
    private void AttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!ReferenceEquals(weapons[activeSlot], null))
            {
                attacking = true;
                StartCoroutine(weapons[activeSlot].Attack(weaponInstances[activeSlot]));
            }
        }
    }

    //Add a weapon to empty slot
    public void AddWeapon(string weapon)
    {
        int assignedSlot = -1;
        Weapon newWeapon = weaponFactory.CreateWeapon(weapon);

        if (ReferenceEquals(weapons[activeSlot], null))
        {
            weapons[activeSlot] = newWeapon;
            assignedSlot = activeSlot;
        }
        else
        {
            for (int i = 0; i < slotsAmount; i++)
            {
                if (ReferenceEquals(weapons[i], null))
                {
                    weapons[i] = newWeapon;
                    assignedSlot = i;
                    break;
                }
            }
        }
        if (assignedSlot == -1)
        {
            DropWeapon(activeSlot);
            weapons[activeSlot] = newWeapon;
            assignedSlot = activeSlot;
        }
        
        weapons[assignedSlot].WeaponObject.GetComponent<SpriteRenderer>().sprite = weapons[assignedSlot].HotbarIcon;
        GameObject weaponIcon = new GameObject(weapon + " Icon");

        RectTransform rectTransform = weaponIcon.AddComponent<RectTransform>();
        rectTransform.SetParent(canvas.transform);

        Image sprite = weaponIcon.AddComponent<Image>();
        sprite.sprite = weapons[assignedSlot].HotbarIcon;
        rectTransform.sizeDelta = new Vector2(sprite.pixelsPerUnit * (slotsSize * 3.5f), sprite.pixelsPerUnit * (slotsSize * 3.5f));
        rectTransform.localPosition = slots[assignedSlot].transform.localPosition;

        /* For later
        if (weapons[assignedSlot] is RangedWeapon)
        {
            ((RangedWeapon)weapons[assignedSlot]).Projectile.GetComponent<BoxCollider2D>().size = weapons[assignedSlot].HotbarIcon.rect.size / weapons[assignedSlot].HotbarIcon.pixelsPerUnit * weaponColliderSize;
        }*/

        slotIcons[assignedSlot] = weaponIcon;
        ActiveSlot(assignedSlot);
    }

    //Initialize weapon in active slot
    private void ActiveSlot(int slot)
    {
        DeactivateSlot(activeSlot);
        activeSlot = slot;
        slots[activeSlot].GetComponent<Image>().sprite = activeSlotIcon;
        if (!ReferenceEquals(weapons[slot], null))
        {
            if (weaponInstances[slot] == null)
            {
                weaponInstances[slot] = Instantiate(weapons[slot].WeaponObject, weaponHolder.transform);

                //Set the size of the collision box the size of the sprite
                if (weapons[slot] is MeleeWeapon)
                {
                    weaponInstances[slot].GetComponent<BoxCollider2D>().size = (weaponInstances[slot].GetComponent<SpriteRenderer>().sprite.rect.size / weaponInstances[slot].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) * weaponColliderSize;

                    ParticleSystem particleSystem = weaponInstances[slot].GetComponentInChildren<ParticleSystem>();
                    var shape = particleSystem.shape;

                    //Take the size of one side of the sprite model and use pythagoras to find the diagonal
                    float weaponDiagonal = Mathf.Sqrt(Mathf.Pow((weaponInstances[slot].GetComponent<SpriteRenderer>().sprite.rect.size / weaponInstances[slot].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit).x, 2) * 2);
                    Debug.Log(weaponDiagonal);
                    shape.scale = new Vector2(weaponDiagonal, 0);

                    //Take the constant between the scale and the radius of the shape:  2.828
                    shape.position = new Vector2(weaponDiagonal / 2.828f, weaponDiagonal / 2.828f);

                    particleSystem.gameObject.SetActive(false);
                }
                    
            }
                
            else
                weaponInstances[slot].SetActive(true);
        }
    }

    //Remove weapon model from slot 
    private void DeactivateSlot(int slot)
    {
        slots[slot].GetComponent<Image>().sprite = nonActiveSlotIcon;
        if (weaponInstances[slot] != null)
        {
            weaponInstances[slot].SetActive(false);
        }
    }

    //Remove slot
    private void RemoveSlotIcon(int slot)
    {
        Destroy(slotIcons[slot]);
    }

    //Remove wepon from inventory and drop it
    private void DropWeapon(int slot)
    {
        if (!ReferenceEquals(weapons[slot], null))
        {
            
            GameObject droppedWeapon = Instantiate(pickUpTemplate, transform.position + new Vector3(0,1, 0), quaternion.identity);

            droppedWeapon.name = (weapons[slot].Name + " Pickup");
            droppedWeapon.GetComponent<SpriteRenderer>().sprite = weapons[activeSlot].HotbarIcon;
            droppedWeapon.GetComponent<PickUpAble>().weaponType = weapons[slot].Name;
            droppedWeapon.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-dropForce, dropForce), dropForce), ForceMode2D.Impulse);

            droppedWeapon.transform.localScale = Vector2.one * pickUpSize;
            RemoveWeapon(slot);
        }
    }

    //Remove weapon from inventory slot
    public void RemoveWeapon(int slot)
    {
        weapons[slot] = null;
        RemoveSlotIcon(slot);
        if (weaponInstances[slot] != null)
        {
            Destroy(weaponInstances[slot]);
            weaponInstances[slot] = null;
        }
    }
}




