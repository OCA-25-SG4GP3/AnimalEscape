using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuInput : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "PanelPuzzle1";
    [SerializeField] private Button[] buttons;
    InputSystem InputSystem = new();
    int selectedBtnIdx = 0;

    // Start is  called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnEnable()
    {
        InputSystem.Enable();
        InputSystem.UI.Submit.performed += OnSubmitPressed;
        InputSystem.UI.Cancel.performed += OnCancelPressed;
        //InputSystem.UI.dir.performed += ;
    }
    void OnDisable()
    {
        InputSystem.UI.Submit.performed -= OnSubmitPressed;
        InputSystem.UI.Cancel.performed -= OnCancelPressed;
        InputSystem.Disable();
    }
    void OnSubmitPressed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        buttons[selectedBtnIdx].onClick.Invoke();
        //ButtonSceneChanger.ChangeScene(nextSceneName);
    }


    void OnCancelPressed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Debug.Log("CANCEL PRESSED");
    }
}
