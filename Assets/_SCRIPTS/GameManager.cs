using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantener entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MENU");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ResetAllCoinProgress();
        }
    }
    void Start()
    {
        score = PlayerPrefs.GetInt("Score", 0); // Cargar score guardado
    }

    public void AddScore(int amount)
    {
        score += amount;
        PlayerPrefs.SetInt("Score", score);
        Debug.Log("Score: " + score);
    }

    private void OnEnable()
    {
        Coin.OnCoinCollected -= AddScore;
        Coin.OnCoinCollected += AddScore;
    }

    private void OnDisable()
    {
        Coin.OnCoinCollected -= AddScore;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ResetAllCoinProgress()
    {
        PlayerPrefs.DeleteAll();
        score = 0; // <-- Esto actualiza el valor en memoria
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.Save(); // Guarda inmediatamente los cambios
        Debug.Log("Progreso reiniciado. Score: " + score);
    }


    public int Score => score; // Exposición segura
}
