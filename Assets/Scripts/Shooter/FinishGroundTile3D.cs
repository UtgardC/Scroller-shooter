using UnityEngine;

public class FinishGroundTile3D : MonoBehaviour
{
    [SerializeField] private Transform stopPoint;
    [SerializeField] private float stopOffsetZ;

    public float StopZ
    {
        get
        {
            Transform target = stopPoint != null ? stopPoint : transform;
            return target.position.z + stopOffsetZ;
        }
    }
}
