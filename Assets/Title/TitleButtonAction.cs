using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleButtonAction : MonoBehaviour
{
    [SerializeField] private string _loadGameScene;
    [SerializeField] private string _loadCreditScene;
    public int _delay; //遅延させたい秒数

    void Awake()
    {
        unselectedColor = menuItems[0].GetComponent<UnityEngine.UI.Image>().color;
        Time.timeScale = 1f;
        selectedIndex = 0;
        UpdateMenuHighlight();
    }

    public void TimeLag()
    {
        Invoke("SceneChange", _delay);
    }

    public void SceneChange(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    //スタートボタンが押されると
    public void StartOnClick()
    {
        SceneChange(_loadGameScene);
        Debug.Log("ゲーム画面へ!");  // ログを出力
    }
    //クレジットボタンが押されると
    public void CreditOnClick()
    {
        SceneChange(_loadCreditScene);
        Debug.Log("クレジット画面へ!");  // ログを出力
    }
    //エンドボタンが押されると
    public void EndOnClick()
    {
        Debug.Log("ゲームを終了!");  // ログを出力
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
    //MENU

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 navigationInput = context.ReadValue<Vector2>();
        NavigateMenu(navigationInput);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return; // only trigger on performed
        SubmitSelection();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed) { } //Close sfx menu, etc
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
    Color unselectedColor;
    private void UpdateMenuHighlight()
    {
        if (menuItems == null) return;

        for (int i = 0; i < menuItems.Length; i++)
        {
            if (menuItems[i] == null) continue; // skip destroyed items
            var img = menuItems[i].GetComponent<UnityEngine.UI.Image>();
            if (img == null) continue;
            img.color = (i == selectedIndex) ? Color.yellow : unselectedColor;
        }
    }
}
