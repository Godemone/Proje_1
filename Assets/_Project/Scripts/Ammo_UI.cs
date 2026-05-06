using TMPro;
using UnityEngine;

public class Ammo_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player_Shooter shooter;
    [SerializeField] private TextMeshProUGUI ammoText;

    private void Update()
    {
        if (shooter == null || ammoText == null)
            return;

        ammoText.text = shooter.CurrentAmmo + " / " + shooter.MaxAmmo;
    }
}