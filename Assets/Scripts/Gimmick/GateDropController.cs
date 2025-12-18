using UnityEngine;

public class GateDropController : MonoBehaviour
{
    public float startY = 10f;     // vị trí ban đầu của cổng
    public float endY = 0f;        // vị trí khi mở hoàn toàn
    public float partialStepPercentage = 0.1f; // mỗi lần hạ trước khi hoàn thành
    public float dropSpeed = 4f;
    public float finalDropSpeed = 10f;

    private bool fullyOpened = false;

    // Drop một bước, nhưng nếu đây là lần cuối (finalStep = true) thì hạ 100%
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