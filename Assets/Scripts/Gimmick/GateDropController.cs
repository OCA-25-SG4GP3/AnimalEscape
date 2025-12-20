using UnityEngine;

public class GateDropController : MonoBehaviour
{
    public float startY = 10f;
    public float endY = 0f;
    public float partialStepPercentage = 0.1f;
    public float dropSpeed = 4f;

    private bool fullyOpened = false;
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
    }

    public void DropStep(bool finalStep = false)
    {
        if (fullyOpened) return;

        float targetY;

        if (finalStep)
        {
            targetY = endY;
            fullyOpened = true;

            // 🔴 Disable collider when fully opened
            if (boxCollider != null)
                boxCollider.enabled = false;
        }
        else
        {
            targetY = Mathf.Max(
                transform.position.y - (startY - endY) * partialStepPercentage,
                endY
            );
        }

        StopAllCoroutines();
        StartCoroutine(SmoothMove(targetY));
    }

    private System.Collections.IEnumerator SmoothMove(float targetY)
    {
        Vector3 pos = transform.position;

        while (Mathf.Abs(pos.y - targetY) > 0.01f)
        {
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * dropSpeed);
            transform.position = pos;
            yield return null;
        }

        pos.y = targetY;
        transform.position = pos;
    }
}
