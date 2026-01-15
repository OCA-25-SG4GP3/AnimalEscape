using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] public bool hasCaught = false;
    [SerializeField] public Transform ropePointT;

    // ジャンプ後、ボタンを踏める残り時間
    public float stepableTimer = 0f;

    // ボタン側が見る用
    public bool CanStepButton => stepableTimer > 0f;

    void Update()
    {
        // タイマーを減らす
        if (stepableTimer > 0f)
        {
            stepableTimer -= Time.deltaTime;
        }
    }


    static public void UpdateCDText(TMP_Text cooldownText, Cooldown skillCD)
    {
        float cdRemaining = skillCD.GetCooldownRemainingSecond();
        if (cdRemaining > 0) cooldownText.text = cdRemaining.ToString("F2");
        else cooldownText.text = "";
        cooldownText.gameObject.GetComponent<TMP_CharBounce>().SetOriginalText(cooldownText.text);
    }

    private float lastY;
    private bool isFalling = false;
    void Start()
    {
        lastY = transform.position.y;
    }

    void FixedUpdate()
    {
        FixedUpdateIsFalling();
    }
    public void SetCaught()
    {
        hasCaught = true;

        GetComponent<AnimalControlSimple>().SetStunnedState(999.0f); //プレイヤー操作を無効化
    }

    private void FixedUpdateIsFalling()
    {
        float currentY = transform.position.y;
        float diff = currentY - lastY;
        float minFall = 0.001f; // ignore tiny movement
        isFalling = diff < -minFall;
        lastY = currentY;
    }

    public bool IsFallingDown() => isFalling;
}