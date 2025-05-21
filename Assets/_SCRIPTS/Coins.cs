using UnityEngine;
using System;

public class Coin : MonoBehaviour
{
    public static event Action<int> OnCoinCollected;

    [SerializeField] private int coinValue = 1;
    [SerializeField] private string coinID;

    private void Start()
    {
        if (string.IsNullOrEmpty(coinID))
        {
            Debug.LogWarning("CoinID not set! Coin will always respawn.");
            return;
        }

        if (PlayerPrefs.GetInt("Coin_" + coinID, 0) == 1)
        {
            gameObject.SetActive(false); // Hide permanently if already collected
        }
        else
        {
            gameObject.SetActive(true); // Keep active if not collected
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameObject.activeSelf) return;

        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetInt("Coin_" + coinID, 1);
            PlayerPrefs.Save();

            OnCoinCollected?.Invoke(coinValue);
            gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    // Only runs in the Unity Editor
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(coinID))
        {
            coinID = Guid.NewGuid().ToString(); // Generates ONCE
            UnityEditor.EditorUtility.SetDirty(this); // Marks object as changed
        }
    }
#endif
}
