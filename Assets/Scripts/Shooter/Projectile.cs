using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifeTime = 3f;

    private float deathTime;

    private void OnEnable()
    {
        deathTime = Time.time + lifeTime;
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Time.time >= deathTime)
        {
            Destroy(gameObject);
        }
    }
}
