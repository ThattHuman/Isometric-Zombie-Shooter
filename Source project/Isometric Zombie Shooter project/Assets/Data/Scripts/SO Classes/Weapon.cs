using UnityEngine;

/// <summary> Weapon that can be equiped and used by Player </summary>
[CreateAssetMenu(fileName = "New Weapon", menuName = "Custom Objects/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] protected string weaponName = "";
    [SerializeField] protected float damage = 5f;
    [SerializeField] protected float shootDelayInSeconds = 1f;      // delay after every shot
    [SerializeField] protected int clipSize = 15;
    [SerializeField] protected float reloadSpeedInSeconds = 15f;    // delay after ammo is being empty

    private int ammo = 1;               // pointer to left ammos
    private bool readyToFire = true;    // need to prevent shoot why in cooldown

    public delegate void WeaponHandler(float dmg, float delay, bool isReloading);  
    public event WeaponHandler Shoot;   // shoot event for other classes

    #region Properties
        public string WeaponName
        {
            get
            {
                return weaponName;
            }
        }
        public float Damage
        {
            get
            {
                return damage;
            }
        }
        public float ShootDelayInSeconds
        {
            get
            {
                return shootDelayInSeconds;
            }
        }
        public int ClipSize
        {
            get
            {
                return clipSize;
            }
        }
        public int Ammo
        {
            get
            {
                return ammo;
            }
        }
        public float ReloadSpeedInSeconds
        {
            get
            {
                return reloadSpeedInSeconds;
            }
        }
    #endregion

    /// <summary> Pool the weapon's trigger for some action </summary>
    public void PoolTheTrigger(MonoBehaviour delayMaker)
    {
        if(readyToFire) // need to prevent shoot why in cooldown
        {
            readyToFire = false;
            ammo--;
            if(ammo == 0)   // if empty
            {
                Shoot(damage, reloadSpeedInSeconds, true);  // will reload
            }   
            else
                Shoot(damage, shootDelayInSeconds, false);  // no reload required, just delay
            
        }
    }

    /// <summary> Restore clip of weapon </summary>
    public void RestoreAmmo()
    {
        ammo = clipSize;
    }

    /// <summary> Enable shoot after delay </summary>
    public void PrepareToFire()
    {
        readyToFire = true;
    }
}
