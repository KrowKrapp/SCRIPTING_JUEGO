using UnityEngine;

public class Trap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("El jugador tocó una trampa");
           

            GameManager.instance.RestartLevel();    
        }
    }
}
