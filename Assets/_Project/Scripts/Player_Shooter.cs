using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 5f;

    private InputAction fireAction;
    private float nextFireTime;

    private void Awake()
    {
        Debug.Log("Shooter Awake");

        if (inputActions == null)
        {
            Debug.LogError("InputActions asset is NOT assigned!");
            return;
        }

        var playerMap = inputActions.FindActionMap("Player", true);
        fireAction = playerMap.FindAction("Fire", true);

        Debug.Log("Found map: " + playerMap.name);
        Debug.Log("Found action: " + fireAction.name);
    }

    private void OnEnable()
    {
        fireAction?.Enable();
        Debug.Log("Fire action enabled");
    }

    private void OnDisable()
    {
        fireAction?.Disable();
    }

    private void Update()
    {
        if (fireAction == null)
            return;

        if (fireAction.triggered && Time.time >= nextFireTime)
        {
            Debug.Log("FIRE TRIGGERED");
            Shoot();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    private void Shoot()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera is NOT assigned!");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
        }
        else
        {
            Debug.Log("Miss");
            Debug.DrawRay(ray.origin, ray.direction * range, Color.white, 1f);
        }
    }
}