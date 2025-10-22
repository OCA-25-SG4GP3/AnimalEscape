using UnityEngine;
using TMPro;

public class TMP_CharBounce : MonoBehaviour
{
    [SerializeField] TMP_Text tmpText;
    [SerializeField] float amplitude = 5f;   // vertical bounce in points
    [SerializeField] float speed = 5f;       // speed of the bounce

    string originalText;
    public void SetOriginalText(string _originalText)
    {
        this.originalText = _originalText;
    }

    void Awake()
    {
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
        originalText = tmpText.text;
    }

    void Update()
    {
        string newText = "";
        for (int i = 0; i < originalText.Length; i++)
        {
            char c = originalText[i];
            if (c == ' ')
            {
                newText += " ";
                continue;
            }

            float offset = Mathf.Sin(Time.time * speed + i) * amplitude;
            newText += $"<voffset={offset}>{c}</voffset>";
        }

        tmpText.text = newText;
    }
}
