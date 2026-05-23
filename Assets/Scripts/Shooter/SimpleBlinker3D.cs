using System.Collections;
using UnityEngine;

public class SimpleBlinker3D : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float interval = 0.1f;

    private Coroutine blinkRoutine;
    private bool[] originalEnabledStates;

    private void Reset()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnDisable()
    {
        StopBlinking();
    }

    private void OnValidate()
    {
        duration = Mathf.Max(0f, duration);
        interval = Mathf.Max(0.01f, interval);
    }

    public void Blink()
    {
        Blink(duration);
    }

    public void Blink(float blinkDuration)
    {
        StopBlinking();

        if (renderers == null || renderers.Length == 0 || blinkDuration <= 0f)
        {
            return;
        }

        CaptureRendererStates();
        blinkRoutine = StartCoroutine(BlinkRoutine(blinkDuration));
    }

    public void StopBlinking()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        RestoreRendererStates();
    }

    private IEnumerator BlinkRoutine(float blinkDuration)
    {
        float endTime = Time.time + blinkDuration;
        bool visible = false;

        while (Time.time < endTime)
        {
            SetRenderersVisible(visible);
            visible = !visible;
            yield return new WaitForSeconds(interval);
        }

        blinkRoutine = null;
        RestoreRendererStates();
    }

    private void CaptureRendererStates()
    {
        originalEnabledStates = new bool[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalEnabledStates[i] = renderers[i] != null && renderers[i].enabled;
        }
    }

    private void SetRenderersVisible(bool visible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer targetRenderer = renderers[i];

            if (targetRenderer == null)
            {
                continue;
            }

            bool originalState = originalEnabledStates == null ||
                                 i >= originalEnabledStates.Length ||
                                 originalEnabledStates[i];

            targetRenderer.enabled = visible && originalState;
        }
    }

    private void RestoreRendererStates()
    {
        if (renderers == null || originalEnabledStates == null)
        {
            return;
        }

        for (int i = 0; i < renderers.Length && i < originalEnabledStates.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled = originalEnabledStates[i];
            }
        }

        originalEnabledStates = null;
    }
}
