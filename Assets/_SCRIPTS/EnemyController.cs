using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2f;
    public Transform pointA, pointB;
    private Vector3 target;

    void Start()
    {
        target = pointB.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, pointA.position) < 0.1f)
            target = pointB.position;
        else if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
            target = pointA.position;
    }
}
