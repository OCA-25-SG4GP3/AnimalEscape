using UnityEngine;

public class GateDropController : MonoBehaviour
{
    [SerializeField] public float startY = 10f;
    [SerializeField] public float endY = 0f;
    [SerializeField] public float partialStepPercentage = 0.1f;
    [SerializeField] public float dropSpeed = 4f;
    [SerializeField] private float finalDropSpeed = 5.0f;

    private bool fullyOpened = false;
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
    }

    public void DropStep(bool finalStep = false, System.Action onComplete = null)
    {
        if (fullyOpened) return;

        float targetY;
        if (finalStep)
        {
            targetY = endY;
            fullyOpened = true;

            // Lock players immediately before gate finishes moving
            var playerDistManager = FindAnyObjectByType<PlayerDistanceManager>();
            if (playerDistManager)
            {
                var player1AnimalControl = playerDistManager.Player1.GetComponent<AnimalControlSimple>();
                var player2AnimalControl = playerDistManager.Player2.GetComponent<AnimalControlSimple>();
                // Lock player inputs
                if (player1AnimalControl) player1AnimalControl.LockInput();
                if (player2AnimalControl) player2AnimalControl.LockInput();
            }

            if (boxCollider != null)
                boxCollider.enabled = false;
        }
        else
        {
            targetY = Mathf.Max(transform.position.y - (startY - endY) * partialStepPercentage, endY);
        }

        StopAllCoroutines();
        StartCoroutine(SmoothMove(targetY, finalStep, onComplete));
    }

    private System.Collections.IEnumerator SmoothMove(float targetY, bool isFinalStep, System.Action callback)
    {
        Vector3 pos = transform.position;
        float speed = isFinalStep ? finalDropSpeed : dropSpeed;

        while (Mathf.Abs(pos.y - targetY) > 0.01f)
        {
            if (isFinalStep)
            {
                pos.y = Mathf.MoveTowards(pos.y, targetY, Time.deltaTime * speed);
            }
            else
            {
                pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * speed);
            }
            transform.position = pos;
            yield return null;
        }
        pos.y = targetY;
        transform.position = pos;

        if (callback != null) callback();
    }

}
