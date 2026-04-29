using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Shooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Shooting")]
    [SerializeField] private float fireRate = 5f;
    [SerializeField] private float range = 100f;

    private InputAction fireAction;
    private float nextFireTime;

    private void Awake()
    {
        var playerMap = inputActions.FindActionMap("Player", true);
        fireAction = playerMap.FindAction("Fire", true);
    }

    private void OnEnable()
    {
        fireAction?.Enable();
    }

    private void OnDisable()
    {
        fireAction?.Disable();
    }

    private void Update()
    {
        if (fireAction == null)
            return;

        if (fireAction.IsPressed() && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    private void Shoot()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * range, Color.white, 1f);
        }
    }
}