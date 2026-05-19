using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement3D : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private BoxCollider movementBounds;
    [SerializeField] private bool lockY = true;
    [SerializeField] private float fixedY;

    private bool actionEnabledByThis;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        fixedY = transform.position.y;
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void OnEnable()
    {
        InputAction action = GetMoveAction();

        if (action != null && !action.enabled)
        {
            action.Enable();
            actionEnabledByThis = true;
        }
    }

    private void OnDisable()
    {
        InputAction action = GetMoveAction();

        if (action != null && actionEnabledByThis)
        {
            action.Disable();
        }

        actionEnabledByThis = false;
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        Vector2 input = ReadMoveInput();
        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 movement = new Vector3(input.x, 0f, input.y) * (moveSpeed * Time.fixedDeltaTime);
        Vector3 targetPosition = rb.position + movement;

        if (lockY)
        {
            targetPosition.y = fixedY;
        }

        targetPosition = ClampToMovementBounds(targetPosition);
        rb.MovePosition(targetPosition);
    }

    private Vector2 ReadMoveInput()
    {
        InputAction action = GetMoveAction();
        return action != null ? action.ReadValue<Vector2>() : Vector2.zero;
    }

    private InputAction GetMoveAction()
    {
        return moveAction != null ? moveAction.action : null;
    }

    private Vector3 ClampToMovementBounds(Vector3 position)
    {
        if (movementBounds == null)
        {
            return position;
        }

        Bounds bounds = movementBounds.bounds;
        position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        position.z = Mathf.Clamp(position.z, bounds.min.z, bounds.max.z);
        return position;
    }
}
