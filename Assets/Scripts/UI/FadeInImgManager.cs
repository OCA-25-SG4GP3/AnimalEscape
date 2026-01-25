using UnityEngine;
using UnityEngine.UI;

public class FadeInImgManager : MonoBehaviour
{
    Image img;
    [SerializeField] private float fadeSpeed = 0.65f; // Alpha units per second
    [SerializeField] private float fadeinDeray = 30.0f; //フェードインに入るまでのディレイ時間
    [SerializeField] private float time = 0.0f;

    void Start()
    {
        img = GetComponent<Image>();
        if (!img.enabled) img.enabled = true;
    }

    void Update()
    {
        if (GameClearManager.FadeOut == true)
        {
            Color c = img.color;
            c.a += (fadeSpeed * 2) * Time.unscaledDeltaTime;
            if (c.a >= 1)
            {
                c.a = 1;
            }
            img.color = c;
        }

        else
        {
            time++;

            if (time >= 30.0f)
            {
                Color c = img.color;
                c.a -= fadeSpeed * Time.unscaledDeltaTime;
                if (c.a <= 0)
                {
                    c.a = 0;
                }
                img.color = c;
            }
        }
    }
}
