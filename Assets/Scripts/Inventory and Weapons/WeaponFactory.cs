using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

//Work animations and attacks in this class
//get projectile for range

//Melee weapons use animation for attack
//Ranged weapons spawn projectile 

//Weapon => Ranged Weapon (Bow and Bottle) => Individual Weapon


//Everytime the weapon is created you need to re get the animator component


//Abstract weapon class for basic variables and methods
public abstract class Weapon : MonoBehaviour
{
    public Transform Player;
    public Inventory inventory;
    public GameObject WeaponObject;
    public Sprite HotbarIcon;
    public string Name;
    public int Damage;
    public int Durability;
    public Weapon(string name, GameObject weaponObject, Sprite hotbarIcon, int damage, int durability)
    {
        Player = GameObject.Find("Player").transform;
        inventory = FindFirstObjectByType<Inventory>();
        Name = name;
        WeaponObject = weaponObject;
        Damage = damage;
        WeaponObject = weaponObject;
        HotbarIcon = hotbarIcon;


    }
    public abstract IEnumerator Attack(GameObject weaponInstance);

    public abstract void OnBreak();
}


//Ranged weapon type
public abstract class RangedWeapon : Weapon
{
    public GameObject Projectile;
    public float ProjectileSpeed;
    public RangedWeapon(string name, GameObject weaponObject, Sprite hotbarIcon, GameObject projectile, int damage, float projectileSpeed, int durability) : base(name, weaponObject, hotbarIcon, damage, durability)
    {
        Projectile = projectile;
        Projectile.GetComponent<PlayerProjectile>().damage = damage;
        weaponObject.GetComponent<Attack>().durability = durability;

        ProjectileSpeed = projectileSpeed;
    }

    public override IEnumerator Attack(GameObject weaponInstance)
    {
        
        weaponInstance.GetComponent<Animation>().Play();

        GameObject projectileInstance = Instantiate(Projectile, weaponInstance.transform.position, Player.transform.rotation);

        projectileInstance.GetComponent<Rigidbody2D>().AddForce(Player.transform.right * ProjectileSpeed);

        projectileInstance.transform.localRotation = Quaternion.Euler(0f, 0f, -45f);

        if (Player.transform.rotation.y != 0)
            projectileInstance.transform.localRotation = Quaternion.Euler(0f, 180f, -45);


        weaponInstance.GetComponent<Attack>().durability--;


        yield return null;

        inventory.attacking = false;

        if (weaponInstance.GetComponent<Attack>().durability <= 0){
            OnBreak();
        }
    }
}

//Melee weapon type
public abstract class MeleeWeapon : Weapon
{
    public MeleeWeapon(string name, GameObject weaponObject, Sprite hotbarIcon, int damage, int durability) : base(name, weaponObject, hotbarIcon, damage, durability)
    {
        weaponObject.GetComponent<Attack>().damage = damage;
        weaponObject.GetComponent<Attack>().durability = durability;
    }

    public override IEnumerator Attack(GameObject weaponInstance)
    {
        inventory.attacking = true;
        weaponInstance.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(weaponInstance.GetComponent<Animation>().clip.length);

        inventory.attacking = false;
        if(weaponInstance.GetComponent<Attack>().durability <= 0)
        {
            OnBreak();
        }
    }   
}

public class Bow : RangedWeapon
{
    public Bow(string name, GameObject weaponObject, Sprite hotbarIcon, GameObject projectile, int damage, float projectileSpeed ,int durability) : base(name, weaponObject, hotbarIcon, projectile, damage, projectileSpeed, durability) { }

    public override void OnBreak()
    {
        inventory.RemoveWeapon(inventory.activeSlot);
        inventory.AddWeapon("Wood");
        inventory.AddWeapon("Rope");

    }
}

public class Bottle : RangedWeapon
{
    
    public Bottle(string name, GameObject weaponObject, Sprite hotbarIcon, GameObject projectile, int damage, float projectileSpeed, int durability) : base(name, weaponObject, hotbarIcon, projectile, damage, projectileSpeed, durability) { }

    public override void OnBreak()
    {
        inventory.RemoveWeapon(inventory.activeSlot);
    }
}

public class Sword : MeleeWeapon
{
    public Sword(string name, GameObject weaponObject, Sprite hotbarIcon, int damage, int durability) : base(name, weaponObject, hotbarIcon, damage, durability) { }

    public override void OnBreak()
    {
        inventory.RemoveWeapon(inventory.activeSlot);
        inventory.AddWeapon("Half Sword");
    }
}

public class HalfSword : MeleeWeapon
{
    public HalfSword(string name, GameObject weaponObject, Sprite hotbarIcon, int damage, int durability) : base(name, weaponObject, hotbarIcon, damage, durability) { }

    public override void OnBreak()
    {
        inventory.RemoveWeapon(inventory.activeSlot);
        inventory.AddWeapon("Dagger");
    }
}

public class Dagger : MeleeWeapon
{
    public Dagger(string name, GameObject weaponObject, Sprite hotbarIcon, int damage, int durability) : base(name, weaponObject, hotbarIcon, damage, durability) { }

    public override void OnBreak()
    {
        inventory.RemoveWeapon(inventory.activeSlot);
    }
}

public class Arrow : MeleeWeapon
{
    public Arrow(string name, GameObject weaponObject, Sprite hotbarIcon, int damage, int durability) : base(name, weaponObject, hotbarIcon, damage, durability) { }

    public override void OnBreak()
    {
        inventory.RemoveWeapon(inventory.activeSlot);
    }
}

public class Wood : MeleeWeapon
{
    public Wood(string name, GameObject weaponObject, Sprite hotbarIcon, int damage, int durability) : base(name, weaponObject, hotbarIcon, damage, durability) { }

    public override void OnBreak()
    {
        inventory.RemoveWeapon(inventory.activeSlot);
        inventory.AddWeapon("Half Wood");
    }
}

public class HalfWood : MeleeWeapon
{
    public HalfWood(string name, GameObject weaponObject, Sprite hotbarIcon, int damage, int durability) : base(name, weaponObject, hotbarIcon, damage, durability) { }

    public override void OnBreak()
    {
        inventory.RemoveWeapon(inventory.activeSlot);
    }
}

public class Rope : MeleeWeapon
{
    public Rope(string name, GameObject weaponObject, Sprite hotbarIcon, int damage, int durability) : base(name, weaponObject, hotbarIcon, damage, durability) { }

    public override void OnBreak()
    {
        inventory.RemoveWeapon(inventory.activeSlot);
    }
}


public class WeaponFactory : MonoBehaviour
{
    public GameObject meleeWeapon;
    public GameObject rangeWeapon;

    public Sprite bowIcon;
    public GameObject bowProjectile;

    public Sprite bottleIcon;
    public GameObject bottleProjectile;

    public Sprite arrowIcon;

    public Sprite ropeIcon;

    public Sprite woodIcon;

    public Sprite halfWoodIcon;

    public Sprite swordIcon;

    public Sprite halfSwordIcon;

    public Sprite daggerIcon;

    public Weapon CreateWeapon(string weapon)
    {
        switch (weapon)
        {
            //Change new to whatever mono behavior uses
            case "Bow":
                return new Bow("Bow", rangeWeapon, bowIcon, bowProjectile, 2, 500,3);
            case "Bottle":
                return new Bottle("Bottle", rangeWeapon, bottleIcon, bottleProjectile, 3, 50, 1);
            case "Arrow":
                return new Arrow("Arrow", meleeWeapon, arrowIcon, 1,  1);
            case "Rope":
                return new Rope("Rope", meleeWeapon, ropeIcon, 1,  1);
            case "Wood":
                return new Wood("Wood", meleeWeapon, woodIcon, 2,  1);
            case "Half Wood":
                return new HalfWood("Half Wood", meleeWeapon, halfWoodIcon, 2, 1);
            case "Sword":
                return new Sword("Sword", meleeWeapon, swordIcon, 5,  1);
            case "Half Sword":
                return new HalfSword("Half Sword", meleeWeapon, halfSwordIcon, 3,  1);
            case "Dagger":
                return new Dagger("Dagger", meleeWeapon, daggerIcon, 2,  1);
        }
        return null;
    }
}