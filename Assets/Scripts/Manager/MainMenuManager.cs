using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private Text _progressText;
    [SerializeField, ReadOnly] private float _progress;

    public async void StartGame()
    {
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(1);
        loadingScene.allowSceneActivation = false;

        _loadingScreen.SetActive(true);
        while (!loadingScene.isDone)
        {
            Debug.Log($"Loading progress: {loadingScene.progress * 100}%");
            _progress = loadingScene.progress;

            if (loadingScene.progress >= 0.9f)
            {
                _progress = 1;
                await Task.Delay(1000);

                loadingScene.allowSceneActivation = true;
            }
            await Task.Yield();
        }
    }

    private void Update()
    {
        _loadingBar.value = Mathf.MoveTowards(_loadingBar.value, _progress, Time.deltaTime);
        _progressText.text = string.Format("Loading {0}%", (int)(_loadingBar.value * 100));

    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.ExitPlaymode();
        }
#endif
    }
}
