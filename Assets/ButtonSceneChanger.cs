using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneChanger : MonoBehaviour
{
    static public void ChangeScene(string nextSceneName)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
