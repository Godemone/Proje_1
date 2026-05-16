using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;

    [Header("Combat")]
    public float damage = 25f;
    public float fireRate = 5f;
    public float range = 100f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public int reserveAmmo = 120;
    public float reloadTime = 2f;

    [Header("Visual Recoil")]
    public float recoilKickBack = 0.035f;
    public float recoilKickUp = 1.2f;
    public float recoilReturnSpeed = 14f;

    [Header("Camera Recoil")]
    public float cameraRecoilAmount = 0.45f;

    [Header("Spread")]
    public float baseSpread = 0.0015f;
    public float maxSpread = 0.02f;
    public float spreadIncreasePerShot = 0.003f;
    public float spreadRecoverySpeed = 6f;

    [Header("Muzzle")]
    public float muzzleFlashTime = 0.03f;
}