using System.Collections.Generic;
using UnityEngine;

public class Combat_Room : MonoBehaviour
{
    [Header("Room")]
    [SerializeField] private Collider triggerZone;
    [SerializeField] private GameObject[] doors;

    [Header("Enemies")]
    [SerializeField] private List<Enemy_Health> enemies = new List<Enemy_Health>();

    private bool roomStarted;
    private bool roomCleared;

    private void Awake()
    {
        foreach (Enemy_Health enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDied += HandleEnemyDied;
                enemy.gameObject.SetActive(false);
            }
        }

        OpenDoors();
    }

    private void OnDestroy()
    {
        foreach (Enemy_Health enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDied -= HandleEnemyDied;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (roomStarted || roomCleared)
            return;

        if (!other.CompareTag("Player"))
            return;

        StartRoom();
    }
    
    private void StartRoom()
    {
        roomStarted = true;

        CloseDoors();

        foreach (Enemy_Health enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
            }
        }

        Debug.Log("Combat room started.");
    }

    private void HandleEnemyDied(Enemy_Health deadEnemy)
    {
        enemies.Remove(deadEnemy);

        Debug.Log("Enemy removed. Remaining: " + enemies.Count);

        if (enemies.Count <= 0)
        {
            ClearRoom();
        }
    }

    private void ClearRoom()
    {
        roomCleared = true;

        OpenDoors();

        Debug.Log("Combat room cleared.");
    }

    private void CloseDoors()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
                door.SetActive(true);
        }
    }

    private void OpenDoors()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
                door.SetActive(false);
        }
    }
}