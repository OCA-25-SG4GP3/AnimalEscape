using UnityEngine;
using UnityEngine.InputSystem;

public class GameOverMenuInput : MonoBehaviour
{
    private InputSystem _inputSystem; //automatically finds the file for us. no need to assign
    [SerializeField] private string nextSceneName;
    
    void Awake()
    {
        _inputSystem = new InputSystem();
    }
    
    void OnEnable()
    {
        _inputSystem.Enable();
        
        _inputSystem.UI.Submit.performed += OnSubmitPressed;
     //   _inputSystem.UI.Cancel.performed += OnCancelPressed;
    }
    
    void OnDisable()
    {
        _inputSystem.UI.Submit.performed -= OnSubmitPressed;
   //     _inputSystem.UI.Cancel.performed -= OnCancelPressed;
        _inputSystem.Disable();
    }
    
    void OnSubmitPressed(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        ButtonSceneChanger.ChangeScene(nextSceneName);
    }
    
    // void OnCancelPressed(InputAction.CallbackContext context)
    // {
    //     if(!context.performed) return;
    //     Debug.Log("CANCEL PRESSED");
    // }
}

