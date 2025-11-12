using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    Canvas canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) canvas.enabled = !canvas.enabled;
    }
}
