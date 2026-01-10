using UnityEngine;
using UnityEngine.UI;

public class FadeInImgManager : MonoBehaviour
{
    Image img;
    [SerializeField] private float fadeSpeed = 1.0f; // Alpha units per second

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        Color c = img.color;
        c.a -= fadeSpeed * Time.unscaledDeltaTime;
        img.color = c;
    }
}
