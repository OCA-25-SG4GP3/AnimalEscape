using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameClearInput : MonoBehaviour
{
    [SerializeField] private Button button;
    InputSystem InputSystem;

    void Awake()
    {
        InputSystem = new InputSystem();
    }

    void OnEnable()
    {
        InputSystem.Enable();
        InputSystem.UI.Submit.performed += OnSubmitPressed;

        // Set button to highlighted state
        if (button != null)
        {
            button.OnPointerEnter(null);
        }
    }

    void OnDisable()
    {
        InputSystem.UI.Submit.performed -= OnSubmitPressed;
        InputSystem.Disable();
    }

    void OnSubmitPressed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (button != null)
        {
            button.onClick.Invoke();
        }
    }

}
