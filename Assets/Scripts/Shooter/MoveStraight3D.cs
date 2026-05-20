using UnityEngine;

public class MoveStraight3D : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection = Vector3.back;
    [SerializeField] private float speed = 4f;
    [SerializeField] private Rigidbody rb;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = GetMoveDirection();
        Vector3 movement = direction * (speed * Time.fixedDeltaTime);

        if (rb != null)
        {
            rb.MovePosition(rb.position + movement);
            return;
        }

        transform.position += movement;
    }

    private void OnValidate()
    {
        speed = Mathf.Max(0f, speed);
    }

    private Vector3 GetMoveDirection()
    {
        if (moveDirection.sqrMagnitude <= 0f)
        {
            return Vector3.back;
        }

        return moveDirection.normalized;
    }
}
