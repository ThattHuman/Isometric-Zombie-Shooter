using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Manage Player's weapons </summary>
public class Equipment : MonoBehaviour
{
    [SerializeField] private BulletSpawner bulletSpawner = null;        // link to bullet spawner
    [SerializeField] protected GameObject UIAmmoBar = null;
    [SerializeField] protected GameObject UIAmmoCountText = null;
    [SerializeField] protected GameObject UIWeaponNameText = null;
    [SerializeField] private List<Weapon> weapons = new List<Weapon>(); // contains weapons
    
    private int pointer = 0;
    private Image aBarImage;
    private Text aBarAmmoCount;
    private Text aBarWeaponName;

    private void Awake()
    {
        // initialize UI elements links
        aBarImage = UIAmmoBar.GetComponent<Image>();
        aBarAmmoCount = UIAmmoCountText.GetComponent<Text>();
        aBarWeaponName = UIWeaponNameText.GetComponent<Text>();

        // prepare all weapons
        for(int i = 0; i < weapons.Count; i++)
        {
            weapons[i].RestoreAmmo();
            weapons[i].PrepareToFire();
            weapons[i].Shoot += WeaponShoot;    // subscribe to Shoot event, when triger is pooled
        }
    }

    // UI updating
    private void Update() 
    {
        UIUpdate();
    }

    // unsubscribe from events
    private void OnDestroy() 
    {
        for(int i = 0; i < weapons.Count; i++)
        {
            weapons[i].RestoreAmmo();
            weapons[i].PrepareToFire();
            weapons[i].Shoot -= WeaponShoot;
        }
    }

    /// <summary> Return active weapon link </summary>
    public Weapon GetCurrentWeapon()
    {
        return weapons[pointer];
    }

    /// <summary> Activate next weapon and return its link </summary>
    public Weapon GetNextWeapon()
    {
        pointer++;
        if(pointer >= weapons.Count)    // to start of list
            pointer = 0;

        return weapons[pointer];
    }

    /// <summary> Activate previous weapon and return its link </summary>
    public Weapon GetPreviousWeapon()
    {
        pointer--;
        if(pointer < 0)                 // to end of list
            pointer = weapons.Count - 1;

        return weapons[pointer];
    }

    /// <summary> Spawn bullet and make cooldown </summary>
    public void WeaponShoot(float dmg, float delay, bool isReloading)
    {
        bulletSpawner.SpawnBullet(transform.position, transform.forward, dmg);
        StartCoroutine(Delay(delay, isReloading));
    }

    /// <summary> Handle delay actions with weapon </summary>
    public IEnumerator Delay(float delay, bool isReloading)
    {
        Weapon weapon_temp = GetCurrentWeapon();
        yield return new WaitForSeconds(delay);
        if(isReloading) // restore clip of weapon if its reloading
            weapon_temp.RestoreAmmo();
        weapon_temp.PrepareToFire();
    }

    /// <summary> Update UI Elements linked to Equipment </summary>
    private void UIUpdate()
    {
        Weapon weaponTemp = GetCurrentWeapon();
        aBarImage.fillAmount = (float)weaponTemp.Ammo / (float)weaponTemp.ClipSize;
        aBarAmmoCount.text = weaponTemp.Ammo.ToString() + " / " + weaponTemp.ClipSize.ToString();
        aBarWeaponName.text = weaponTemp.WeaponName;
    }
}