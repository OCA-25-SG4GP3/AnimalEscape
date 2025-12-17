using UnityEngine;
using UnityEngine.Audio;


public class GateDropController : MonoBehaviour
{
    public float startY = 10f;     // vị trí ban đầu của cổng
    public float endY = 0f;        // vị trí khi mở hoàn toàn
    public float partialStepPercentage = 0.1f; // mỗi lần hạ trước khi hoàn thành
    public float dropSpeed = 4f;

    private bool fullyOpened = false;

    public AudioClip gateSound;      // ゲート音のファイル
    private AudioSource audioSource; // AudioSourceを使うための変数


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Drop một bước, nhưng nếu đây là lần cuối (finalStep = true) thì hạ 100%
    public void DropStep(bool finalStep = false)
    {
        if (fullyOpened) return;

        float targetY;
        if (finalStep)
        {
            targetY = endY;
            fullyOpened = true;
        }
        else
        {
            targetY = Mathf.Max(transform.position.y - (startY - endY) * partialStepPercentage, endY);
        }

        StopAllCoroutines();
        StartCoroutine(SmoothMove(targetY));
    }

    private System.Collections.IEnumerator SmoothMove(float targetY)
    {
        Vector3 pos = transform.position;
        audioSource.PlayOneShot(gateSound);
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