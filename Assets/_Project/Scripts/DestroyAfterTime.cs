using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.2f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}