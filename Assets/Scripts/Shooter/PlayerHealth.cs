using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [System.Serializable]
    public class LivesChangedEvent : UnityEvent<int>
    {
    }

    [SerializeField] private int maxLives = 3;
    [SerializeField] private float invulnerabilityDuration = 1.5f;
    [SerializeField] private SimpleBlinker3D blinker;
    [SerializeField] private LivesChangedEvent onLivesChanged = new LivesChangedEvent();
    [SerializeField] private UnityEvent onDeath = new UnityEvent();

    private int currentLives;
    private bool isInvulnerable;
    private bool isDead;
    private Coroutine invulnerabilityRoutine;

    public int CurrentLives => currentLives;
    public int MaxLives => maxLives;
    public bool IsInvulnerable => isInvulnerable;

    private void Reset()
    {
        blinker = GetComponent<SimpleBlinker3D>();
    }

    private void Awake()
    {
        if (blinker == null)
        {
            blinker = GetComponent<SimpleBlinker3D>();
        }
    }

    private void OnEnable()
    {
        currentLives = maxLives;
        isInvulnerable = false;
        isDead = false;
        onLivesChanged?.Invoke(currentLives);
    }

    private void OnDisable()
    {
        if (invulnerabilityRoutine != null)
        {
            StopCoroutine(invulnerabilityRoutine);
            invulnerabilityRoutine = null;
        }

        isInvulnerable = false;

        if (blinker != null)
        {
            blinker.StopBlinking();
        }
    }

    private void OnValidate()
    {
        maxLives = Mathf.Max(1, maxLives);
        invulnerabilityDuration = Mathf.Max(0f, invulnerabilityDuration);
    }

    public void TakeDamage(int amount)
    {
        if (isDead || isInvulnerable || amount <= 0)
        {
            return;
        }

        currentLives = Mathf.Max(0, currentLives - amount);
        onLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            Die();
            return;
        }

        StartInvulnerability();
    }

    public void AddLife(int amount)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        currentLives += amount;
        onLivesChanged?.Invoke(currentLives);
    }

    private void StartInvulnerability()
    {
        if (invulnerabilityRoutine != null)
        {
            StopCoroutine(invulnerabilityRoutine);
        }

        invulnerabilityRoutine = StartCoroutine(InvulnerabilityRoutine());
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        if (blinker != null)
        {
            blinker.Blink(invulnerabilityDuration);
        }

        yield return new WaitForSeconds(invulnerabilityDuration);

        isInvulnerable = false;
        invulnerabilityRoutine = null;
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        isInvulnerable = false;

        if (blinker != null)
        {
            blinker.StopBlinking();
        }

        onDeath?.Invoke();
    }
}
