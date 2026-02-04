using UnityEngine;
using UnityEngine.UI;

public class HighlightGameOverButton : MonoBehaviour
{
    [SerializeField] private Button button;

    void OnEnable()
    {

        // Set button to highlighted state
        if (button != null)
        {
            button.OnPointerEnter(null);
        }
    }
}
