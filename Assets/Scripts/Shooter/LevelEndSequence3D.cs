using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelEndSequence3D : MonoBehaviour
{
    [SerializeField] private ScrollingTileBackground3D scrollingBackground;
    [SerializeField] private GameObject finishTilePrefab;
    [SerializeField] private Transform finishStopTarget;
    [SerializeField] private float delayAfterFinishTileGenerated = 0f;
    [SerializeField] private float decelerationStartDistance = 12f;
    [SerializeField] private float scrollStopDuration = 2f;
    [SerializeField] private PlayerMovement3D playerMovement;
    [SerializeField] private PlayerAutoFire3D playerAutoFire;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Transform playerPoint1;
    [SerializeField] private Transform playerPoint2;
    [SerializeField] private float playerMoveSpeed = 8f;
    [SerializeField] private float waitAtPoint1 = 1f;
    [SerializeField] private UnityEvent onComplete = new UnityEvent();

    private Coroutine endSequenceRoutine;

    private void Awake()
    {
        CachePlayerRigidbody();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        endSequenceRoutine = null;
    }

    private void OnValidate()
    {
        delayAfterFinishTileGenerated = Mathf.Max(0f, delayAfterFinishTileGenerated);
        decelerationStartDistance = Mathf.Max(0f, decelerationStartDistance);
        scrollStopDuration = Mathf.Max(0f, scrollStopDuration);
        playerMoveSpeed = Mathf.Max(0.01f, playerMoveSpeed);
        waitAtPoint1 = Mathf.Max(0f, waitAtPoint1);
    }

    public void StartEndSequence()
    {
        if (endSequenceRoutine != null)
        {
            StopAllCoroutines();
        }

        CachePlayerRigidbody();
        endSequenceRoutine = StartCoroutine(EndSequenceRoutine());
    }

    private IEnumerator EndSequenceRoutine()
    {
        if (scrollingBackground == null || finishTilePrefab == null)
        {
            Debug.LogWarning("LevelEndSequence3D needs a ScrollingTileBackground3D and a finish tile prefab.");
            endSequenceRoutine = null;
            yield break;
        }

        yield return WaitForFinishTileGenerated();

        if (delayAfterFinishTileGenerated > 0f)
        {
            yield return new WaitForSeconds(delayAfterFinishTileGenerated);
        }

        DisablePlayerControls();

        bool scrollDone = false;
        bool playerDone = false;

        StartCoroutine(ScrollEndRoutine(() => scrollDone = true));
        StartCoroutine(PlayerAutopilotRoutine(() => playerDone = true));

        yield return new WaitUntil(() => scrollDone && playerDone);

        endSequenceRoutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator WaitForFinishTileGenerated()
    {
        if (scrollingBackground == null || finishTilePrefab == null)
        {
            yield break;
        }

        scrollingBackground.QueueFinishTile(finishTilePrefab);
        yield return new WaitUntil(() => scrollingBackground.ActiveFinishTile != null);
    }

    private IEnumerator ScrollEndRoutine(System.Action onDone)
    {
        if (scrollingBackground == null || finishStopTarget == null || scrollingBackground.ActiveFinishTile == null)
        {
            onDone?.Invoke();
            yield break;
        }

        FinishGroundTile3D finishTile = scrollingBackground.ActiveFinishTile;

        yield return new WaitUntil(() =>
            finishTile == null ||
            finishTile.StopZ <= finishStopTarget.position.z + decelerationStartDistance);

        scrollingBackground.SmoothStopFinishAt(finishStopTarget, scrollStopDuration);
        yield return new WaitUntil(() => !scrollingBackground.IsSmoothStopping);

        onDone?.Invoke();
    }

    private IEnumerator PlayerAutopilotRoutine(System.Action onDone)
    {
        if (playerPoint1 != null)
        {
            yield return MovePlayerTo(playerPoint1.position);
        }

        if (waitAtPoint1 > 0f)
        {
            yield return new WaitForSeconds(waitAtPoint1);
        }

        if (playerPoint2 != null)
        {
            yield return MovePlayerTo(playerPoint2.position);
        }

        onDone?.Invoke();
    }

    private IEnumerator MovePlayerTo(Vector3 targetPosition)
    {
        Rigidbody targetRb = playerRb;

        if (targetRb == null)
        {
            yield break;
        }

        while ((targetRb.position - targetPosition).sqrMagnitude > 0.0001f)
        {
            Vector3 nextPosition = Vector3.MoveTowards(
                targetRb.position,
                targetPosition,
                playerMoveSpeed * Time.deltaTime);

            targetRb.MovePosition(nextPosition);
            yield return null;
        }

        targetRb.MovePosition(targetPosition);
    }

    private void DisablePlayerControls()
    {
        if (playerAutoFire != null)
        {
            playerAutoFire.enabled = false;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void CachePlayerRigidbody()
    {
        if (playerRb != null)
        {
            return;
        }

        if (playerMovement != null)
        {
            playerRb = playerMovement.GetComponent<Rigidbody>();
            return;
        }

        if (playerAutoFire != null)
        {
            playerRb = playerAutoFire.GetComponent<Rigidbody>();
        }
    }
}
