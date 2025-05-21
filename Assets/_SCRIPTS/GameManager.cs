using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }

    private void OnEnable()
    {
        Coin.OnCoinCollected += AddScore;
    }

    private void OnDisable()
    {
        Coin.OnCoinCollected -= AddScore;
    }

    public void RestartLevel()
    {
        // Reiniciar el nivel actual
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }


}
