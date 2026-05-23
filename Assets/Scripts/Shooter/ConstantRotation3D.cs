using UnityEngine;

public class ConstantRotation3D : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float degreesPerSecond = 90f;
    [SerializeField] private Space relativeTo = Space.Self;

    private void Update()
    {
        Vector3 axis = rotationAxis.sqrMagnitude > 0f ? rotationAxis.normalized : Vector3.up;
        transform.Rotate(axis, degreesPerSecond * Time.deltaTime, relativeTo);
    }
}
