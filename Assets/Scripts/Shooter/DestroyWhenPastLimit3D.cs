using UnityEngine;

public class DestroyWhenPastLimit3D : MonoBehaviour
{
    [SerializeField] private float limitZ = -20f;
    [SerializeField] private bool destroyInsteadOfDisable = true;

    private void Update()
    {
        if (transform.position.z > limitZ)
        {
            return;
        }

        if (destroyInsteadOfDisable)
        {
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
    }
}
