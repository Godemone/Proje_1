using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Shooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Shooting")]
    [SerializeField] private LayerMask hitLayers;

    [Header("Weapons")]
    [SerializeField] private WeaponData[] weapons;
    [SerializeField] private int currentWeaponIndex;

    [Header("Feel")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject hitEffectPrefab;

    [Header("Recoil")]
    [SerializeField] private Transform recoilTransform;

    [Header("Camera Recoil")]
    [SerializeField] private Player_Controller playerController;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;

    private float currentSpread;

    private Vector3 originalRecoilPosition;
    private Quaternion originalRecoilRotation;
    private Vector3 targetRecoilPosition;
    private float currentRecoilRotation;

    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction switchWeaponAction;

    private WeaponData currentWeapon;

    private float nextFireTime;
    private int currentAmmo;
    private int reserveAmmo;
    private bool isReloading;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => currentWeapon != null ? currentWeapon.magazineSize : 0;
    public int ReserveAmmo => reserveAmmo;
    public string CurrentWeaponName => currentWeapon != null ? currentWeapon.weaponName : "None";
    public bool IsReloading => isReloading;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("Input Actions is missing on Player_Shooter.");
            return;
        }

        if (recoilTransform != null)
        {
            originalRecoilPosition = recoilTransform.localPosition;
            originalRecoilRotation = recoilTransform.localRotation;
        }

        var playerMap = inputActions.FindActionMap("Player", true);

        fireAction = playerMap.FindAction("Fire", true);
        reloadAction = playerMap.FindAction("Reload", true);
        switchWeaponAction = playerMap.FindAction("SwitchWeapon", true);

        EquipWeapon(currentWeaponIndex);
    }

    private void OnEnable()
    {
        fireAction?.Enable();
        reloadAction?.Enable();
        switchWeaponAction?.Enable();
    }

    private void OnDisable()
    {
        fireAction?.Disable();
        reloadAction?.Disable();
        switchWeaponAction?.Disable();
    }

    private void Update()
    {
        if (currentWeapon == null)
            return;

        HandleWeaponScroll();

        if (reloadAction != null && reloadAction.triggered)
        {
            StartCoroutine(Reload());
        }

        if (fireAction != null && fireAction.IsPressed() && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / currentWeapon.fireRate);
        }

        HandleRecoil();
        
        HandleSpreadRecovery();
    }

    private void HandleWeaponScroll()
    {
        if (switchWeaponAction == null)
            return;

        if (isReloading)
            return;

        Vector2 scrollValue = switchWeaponAction.ReadValue<Vector2>();

        if (scrollValue.y > 0f)
        {
            SwitchWeapon(1);
        }
        else if (scrollValue.y < 0f)
        {
            SwitchWeapon(-1);
        }
    }

    private void Shoot()
    {
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
            
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentAmmo--;

        ApplyRecoil();

        if (playerController != null)
        {
            playerController.AddCameraRecoil(currentWeapon.cameraRecoilAmount);
        }

        StartCoroutine(ShowMuzzleFlash());

        currentSpread = Mathf.Min(
            currentSpread + currentWeapon.spreadIncreasePerShot,
            currentWeapon.maxSpread
        );

        float spreadX = Random.Range(-currentSpread, currentSpread);
        float spreadY = Random.Range(-currentSpread, currentSpread);

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f + spreadX, 0.5f + spreadY, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, currentWeapon.range, hitLayers))
        {
            Debug.Log("Hit: " + hit.collider.name);

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(currentWeapon.damage);
            }

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * currentWeapon.range, Color.white, 1f);
        }
    }

    private IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        if (currentWeapon == null)
            yield break;

        if (currentAmmo >= currentWeapon.magazineSize)
            yield break;

        if (reserveAmmo <= 0)
            yield break;

        isReloading = true;

        Debug.Log("Reloading...");

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        int neededAmmo = currentWeapon.magazineSize - currentAmmo;
        int ammoToReload = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;

        Debug.Log("Reload complete.");
    }

    private void SwitchWeapon(int direction)
    {
        if (weapons == null || weapons.Length <= 1)
            return;

        currentWeaponIndex += direction;

        if (currentWeaponIndex >= weapons.Length)
            currentWeaponIndex = 0;

        if (currentWeaponIndex < 0)
            currentWeaponIndex = weapons.Length - 1;

        EquipWeapon(currentWeaponIndex);
    }

    private void EquipWeapon(int index)
    {
        if (weapons == null || weapons.Length == 0)
        {
            Debug.LogError("No weapons assigned.");
            return;
        }

        if (index < 0 || index >= weapons.Length)
        {
            Debug.LogError("Invalid weapon index.");
            return;
        }

        currentWeapon = weapons[index];

        currentAmmo = currentWeapon.magazineSize;
        reserveAmmo = currentWeapon.reserveAmmo;

        Debug.Log("Equipped weapon: " + currentWeapon.weaponName);
    }

    private IEnumerator ShowMuzzleFlash()
    {
        if (muzzleFlash == null || currentWeapon == null)
            yield break;

        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(currentWeapon.muzzleFlashTime);
        muzzleFlash.SetActive(false);
    }

    private void ApplyRecoil()
    {
        if (recoilTransform == null || currentWeapon == null)
            return;

        targetRecoilPosition += Vector3.back * currentWeapon.recoilKickBack;
        currentRecoilRotation += currentWeapon.recoilKickUp;
    }

    private void HandleRecoil()
    {
        if (recoilTransform == null || currentWeapon == null)
            return;

        targetRecoilPosition = Vector3.Lerp(
            targetRecoilPosition,
            Vector3.zero,
            Time.deltaTime * currentWeapon.recoilReturnSpeed
        );

        currentRecoilRotation = Mathf.Lerp(
            currentRecoilRotation,
            0f,
            Time.deltaTime * currentWeapon.recoilReturnSpeed
        );

        recoilTransform.localPosition = originalRecoilPosition + targetRecoilPosition;
        recoilTransform.localRotation = originalRecoilRotation * Quaternion.Euler(-currentRecoilRotation, 0f, 0f);
    }

    private void HandleSpreadRecovery()
    {
        if (currentWeapon == null)
            return;

        currentSpread = Mathf.Lerp(
            currentSpread,
            currentWeapon.baseSpread,
            Time.deltaTime * currentWeapon.spreadRecoverySpeed
        );
    }
}