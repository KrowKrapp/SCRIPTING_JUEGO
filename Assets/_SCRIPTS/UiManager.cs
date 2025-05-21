using UnityEngine;
using TMPro;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class UiManager : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] TextMeshProUGUI uitext;

    private void Awake()
    {
        gameManager = GameManager.instance;
        UpdateScore(0);
    }

    private void OnEnable()
    {
        Coin.OnCoinCollected += UpdateScore;
    }
    private void OnDisable()
    {
        Coin.OnCoinCollected -= UpdateScore;
    }

    private void UpdateScore(int obj)
    {
        Debug.Log("Score: " + gameManager.score);
        uitext.text = "Score: " + PlayerPrefs.GetInt("Score", 0);
    }
}
