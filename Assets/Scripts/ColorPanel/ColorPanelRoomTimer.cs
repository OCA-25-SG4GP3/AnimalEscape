using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorPanelRoomTimer : MonoBehaviour
{
    //[SerializeField] private TMP_Text timeText;
    [SerializeField] private float totalTime = 120f; // total seconds, e.g., 2 minutes
    [SerializeField] private float addTimePerRoom = 30f; // total seconds add
    [SerializeField] private float timeBeforeGameOverScreen = 5.0f;
    [SerializeField] private GameObject zookeeperPrefab;
    [SerializeField, Header("ゲームオーバー時、有効にする")] public GameObject gameOverImage;
    [SerializeField] private List<Transform> spawnTs = new();
    //private string originalString = "残り時間 : ";
    bool isGameOver = false;
    [SerializeField] private RectTransform zookeeperIcon;
    [SerializeField] Slider timerSlider;
    float startingTime;
    void Awake()
    {
        startingTime = totalTime;
        timerSlider.minValue = 0;
        timerSlider.maxValue = totalTime; // totalTime = max time
        timerSlider.value = 0;            // start at 0
    }
    void SetSlider()
    {
        timerSlider.value = startingTime - totalTime; // slider increases as time passes
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1)) { Debug.Log("TIME END SET"); SetTimeEnd(); }
#endif
        if (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
            if (totalTime < 0) totalTime = 0;

            //UpdateTimeText();
            SetSlider();
        }
        else
        {
            if (!isGameOver)
            {
                SetTimeEnd();
            }
        }
    }

    // private void UpdateTimeText()
    // {
    //     int minutes = Mathf.FloorToInt(totalTime / 60f);
    //     int seconds = Mathf.FloorToInt(totalTime % 60f);

    //     timeText.text = originalString + minutes.ToString("00") + ":" + seconds.ToString("00");
    // }

    private void ResetSceneByGameOverImpl()
    {
        ButtonSceneChanger.ChangeScene("TemporaryGameOver");
    }

    public void ResetSceneByGameOver()
    {
        if (gameOverImage.activeSelf) return;
        gameOverImage.SetActive(true);
        Invoke("ResetSceneByGameOverImpl", timeBeforeGameOverScreen);
    }

    //終わる時間の設定ができる関数
    //現在は仮で時間が０になったらゲームオーバーにするように変更しています
    private void SetTimeEnd()
    {
        totalTime = 0;
        // UpdateTimeText();

        // timeText.text = "飼育員が来ます！";
        foreach (Transform spawnT in spawnTs)
        {
            if (spawnT == null || zookeeperPrefab == null)
            {
                Debug.LogError("Zookeeper spawn positions are Missing!");
                continue;
            }

            var inst = Instantiate(zookeeperPrefab, spawnT);
            AILogicController aiLogic = inst.GetComponent<AILogicController>();
            aiLogic.SetStateByEnum(AILogicController.SelectedState.InfiniteChase);
        }
        isGameOver = true;
    }


    public void AddTime()
    {
        totalTime += addTimePerRoom;
    }
}
