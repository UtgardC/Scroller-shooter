using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private Camera boundsCamera;
    [SerializeField] private Vector2 boundsPadding = new Vector2(0.5f, 0.5f);

    private void Awake()
    {
        if (boundsCamera == null)
        {
            boundsCamera = Camera.main;
        }
    }

    private void Update()
    {
        Vector2 input = ReadMoveInput();
        Vector3 movement = new Vector3(input.x, 0f, input.y);

        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        transform.position += movement * moveSpeed * Time.deltaTime;
        ClampToCameraBounds();
    }

    private Vector2 ReadMoveInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return Vector2.zero;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            horizontal -= 1f;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            horizontal += 1f;
        }

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            vertical -= 1f;
        }

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            vertical += 1f;
        }

        return new Vector2(horizontal, vertical);
    }

    private void ClampToCameraBounds()
    {
        if (boundsCamera == null || !TryGetCameraBoundsAtPlayerHeight(out Bounds bounds))
        {
            return;
        }

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, bounds.min.x + boundsPadding.x, bounds.max.x - boundsPadding.x);
        position.z = Mathf.Clamp(position.z, bounds.min.z + boundsPadding.y, bounds.max.z - boundsPadding.y);
        transform.position = position;
    }

    private bool TryGetCameraBoundsAtPlayerHeight(out Bounds bounds)
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Vector3 min = new Vector3(float.PositiveInfinity, transform.position.y, float.PositiveInfinity);
        Vector3 max = new Vector3(float.NegativeInfinity, transform.position.y, float.NegativeInfinity);

        if (!TryAddViewportCorner(playerPlane, new Vector3(0f, 0f, 0f), ref min, ref max) ||
            !TryAddViewportCorner(playerPlane, new Vector3(0f, 1f, 0f), ref min, ref max) ||
            !TryAddViewportCorner(playerPlane, new Vector3(1f, 0f, 0f), ref min, ref max) ||
            !TryAddViewportCorner(playerPlane, new Vector3(1f, 1f, 0f), ref min, ref max))
        {
            bounds = default;
            return false;
        }

        bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return true;
    }

    private bool TryAddViewportCorner(Plane plane, Vector3 viewportPoint, ref Vector3 min, ref Vector3 max)
    {
        Ray ray = boundsCamera.ViewportPointToRay(viewportPoint);

        if (!plane.Raycast(ray, out float distance))
        {
            return false;
        }

        Vector3 point = ray.GetPoint(distance);
        min = Vector3.Min(min, point);
        max = Vector3.Max(max, point);
        return true;
    }
}
