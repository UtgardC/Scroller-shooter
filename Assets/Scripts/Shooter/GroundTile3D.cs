using UnityEngine;

public class GroundTile3D : MonoBehaviour
{
    [SerializeField] private float lengthZ = 20f;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    public float LengthZ => Mathf.Max(0.01f, lengthZ);

    public float StartZ
    {
        get
        {
            if (startPoint != null)
            {
                return startPoint.position.z;
            }

            if (endPoint != null)
            {
                return endPoint.position.z - LengthZ;
            }

            return transform.position.z - LengthZ * 0.5f;
        }
    }

    public float EndZ
    {
        get
        {
            if (endPoint != null)
            {
                return endPoint.position.z;
            }

            if (startPoint != null)
            {
                return startPoint.position.z + LengthZ;
            }

            return transform.position.z + LengthZ * 0.5f;
        }
    }

    private void OnValidate()
    {
        lengthZ = Mathf.Max(0.01f, lengthZ);
    }

    public void PlaceStartAt(float targetStartZ)
    {
        float offsetZ = targetStartZ - StartZ;
        transform.position += new Vector3(0f, 0f, offsetZ);
    }
}
