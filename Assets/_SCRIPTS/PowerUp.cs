using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerType { SpeedBoost, ExtraLife }
    public PowerType type;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (type == PowerType.SpeedBoost)
            {
                other.GetComponent<PlayerController>().moveSpeed += 2f;
            }
            else if (type == PowerType.ExtraLife)
            {
                // Aquí podrías incrementar una variable de vidas
                Debug.Log("Vida extra");
            }

            Destroy(gameObject);
        }
    }
}
