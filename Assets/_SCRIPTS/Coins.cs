using UnityEngine;
using System;

public class Coin : MonoBehaviour
{
    public static event Action<int> OnCoinCollected;

    [SerializeField] private int coinValue = 1;
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            // Evento para sonido, UI, etc.
            OnCoinCollected?.Invoke(coinValue);

            // Desactivar objeto en lugar de destruirlo
            gameObject.SetActive(false);
        }
    }

    public void ResetCoin()
    {
        collected = false;
        gameObject.SetActive(true);
    }
}
