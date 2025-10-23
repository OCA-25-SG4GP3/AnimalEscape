using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneChanger : MonoBehaviour
{
    // ‘JˆÚ‚µ‚½‚¢ƒV[ƒ“–¼
    [SerializeField] private string nextSceneName = "NextScene";

    public void ChangeScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
