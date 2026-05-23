using System.Collections;
using UnityEngine;

public class WoobleAnim : MonoBehaviour
{
    [SerializeField] private Vector3 worldAxis = Vector3.forward;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float intensity = 10f;

    private Coroutine woobleRoutine;
    private Quaternion baseRotation;

    private void OnDisable()
    {
        StopWooble();
    }

    private void OnValidate()
    {
        duration = Mathf.Max(0f, duration);
        intensity = Mathf.Max(0f, intensity);
    }

    public void PlayWooble()
    {
        StopWooble();

        if (duration <= 0f || intensity <= 0f)
        {
            return;
        }

        baseRotation = transform.rotation;
        woobleRoutine = StartCoroutine(WoobleRoutine());
    }

    private void StopWooble()
    {
        if (woobleRoutine != null)
        {
            StopCoroutine(woobleRoutine);
            woobleRoutine = null;
            transform.rotation = baseRotation;
        }
    }

    private IEnumerator WoobleRoutine()
    {
        Vector3 axis = worldAxis.sqrMagnitude > 0f ? worldAxis.normalized : Vector3.forward;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float normalizedTime = elapsed / duration;
            float fade = 1f - normalizedTime;
            float angle = Mathf.Sin(normalizedTime * Mathf.PI * 4f) * intensity * fade;

            transform.rotation = Quaternion.AngleAxis(angle, axis) * baseRotation;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = baseRotation;
        woobleRoutine = null;
    }
}
