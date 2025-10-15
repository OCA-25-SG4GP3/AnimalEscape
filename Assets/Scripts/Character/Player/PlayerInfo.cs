using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] public bool hasCaught = false;
    [SerializeField] public Transform ropePointT;
    static public void UpdateCDText(TMP_Text cooldownText, Cooldown skillCD)
    {
        float cdRemaining = skillCD.GetCooldownRemainingSecond();
        if (cdRemaining > 0) cooldownText.text = cdRemaining.ToString("F2");
        else cooldownText.text = "";
        cooldownText.gameObject.GetComponent<TMP_CharBounce>().SetOriginalText(cooldownText.text);
    }
}