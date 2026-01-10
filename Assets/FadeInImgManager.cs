using UnityEngine;
using UnityEngine.UI;

public class FadeInImgManager : MonoBehaviour
{

    Image img;
    //[SerializeField] private float uiAppearSeconds = 3.0f;//UI‚Ì•\Ž¦ŽžŠÔ (•b)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Color c = img.color;
        c.a -= 0.005f;
        img.color = c;
    }
}
