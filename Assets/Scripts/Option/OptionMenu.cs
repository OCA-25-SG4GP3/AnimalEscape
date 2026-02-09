using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public bool IsPaused => canvas.enabled;
    [NonSerializedAttribute] public Canvas canvas;
    [NonSerializedAttribute] private GameStartUI startUI;
    //
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //protected override void Awake()
    void Awake()
    {
        //    base.Awake();
        canvas = GetComponent<Canvas>();
        startUI = GameObject.FindAnyObjectByType<GameStartUI>();

        canvas.enabled = false;
        Time.timeScale = 1f; //Reset time back when the option menu is created
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {        
        //Debug.Log(Time.timeScale);
        //編集:江頭 
        if(!startUI || startUI.isApear == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ToggleOption();
        }        
    }
    public void ToggleOption()
    {
        if (startUI && startUI.isApear) return;

        canvas.enabled = !canvas.enabled;
        bool isPaused = !Time.timeScale.Equals(0f);
        Time.timeScale = isPaused ? 0f : 1f;
        //編集:江頭　以下は岩野さんからのソース改善案のようです
        //bool isPaused = !Time.timeScale.Equals(0f);
        //Time.timeScale = isPaused ? 0f : 1f;
        //canvas.enabled = isPaused ? true : false;


        foreach (var player in PlayerInputManager.I.players)
        {
            if (isPaused)
            {
                player.actions.FindActionMap("Player").Disable();
                player.actions.FindActionMap("UI").Enable();
            }
            else
            {
                player.actions.FindActionMap("Player").Enable();
                player.actions.FindActionMap("UI").Disable();
            }
        }

        selectedIndex = 2; //Return
        UpdateMenuHighlight();
    }

    [SerializeField] private RectTransform[] menuItems; // assign buttons in inspector
    private int selectedIndex = 0;
    private float navCooldown = 0.2f; // prevent super-fast scrolling
    private float lastNavTime = 0f;
    public void SubmitSelection()
    {
        menuItems[selectedIndex].GetComponent<Button>().onClick.Invoke();
    }
    public void NavigateMenu(Vector2 navigationInput)
    {
        // Only allow navigation after cooldown
        if (Time.unscaledTime - lastNavTime < navCooldown) return;

        if (navigationInput.y > 0.5f)
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
            lastNavTime = Time.unscaledTime;
        }
        else if (navigationInput.y < -0.5f)
        {
            selectedIndex = Mathf.Min(menuItems.Length - 1, selectedIndex + 1);
            lastNavTime = Time.unscaledTime;
        }

        // Highlight the selected menu item
        UpdateMenuHighlight();
    }

    private void UpdateMenuHighlight()
    {
        if (menuItems == null) return;

        for (int i = 0; i < menuItems.Length; i++)
        {
            if (menuItems[i] == null) continue; // skip destroyed items
            var img = menuItems[i].GetComponent<UnityEngine.UI.Image>();
            if (img == null) continue;
            img.color = (i == selectedIndex) ? Color.yellow : Color.white;
        }
    }

}
