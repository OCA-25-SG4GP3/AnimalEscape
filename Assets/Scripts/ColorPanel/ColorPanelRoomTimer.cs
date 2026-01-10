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
    [SerializeField, Header("Game Overに出るキャンバスオブジェクト")] public GameObject gameOverImage;
    [SerializeField] private List<Transform> spawnTs = new();
    bool isGameOver = false;
    [SerializeField] private RectTransform zookeeperIcon;
    [SerializeField] Slider timerSlider;
    float startingTime;

    // ============ 追加: Zookeeperがスポーン済みかどうかのフラグ ============
    private bool zookeepersSpawned = false;

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("TIME END SET"); SetTimeEnd();
            GaugeFlickering gaugeFlicker = FindAnyObjectByType<GaugeFlickering>();
            if (gaugeFlicker)
            {
                gaugeFlicker.PlayGaugeFlicker();
            }
        }
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

    public void SetGameOverByOneCaught() //全員捕まえた理由でゲームオーバー
    {
        if (gameOverImage.activeSelf) return;
        gameOverImage.SetActive(true);
        Invoke("ResetSceneByGameOverImpl", timeBeforeGameOverScreen);
    }

    public void SetGameOverByAllCaught() //全員捕まえた理由でゲームオーバー
    {
        if (gameOverImage.activeSelf) return;
        gameOverImage.SetActive(true);
        Invoke("ResetSceneByGameOverImpl", timeBeforeGameOverScreen);
    }

    private void SetTimeEnd()
    {
        totalTime = 0;
        // UpdateTimeText();
        SpawnZookeepers();
        isGameOver = true;
        SetSlider();
    }

    private void SpawnZookeepers()
    {
        // ============ 追加: スポーン済みフラグを立てる ============
        zookeepersSpawned = true;
        // ========================================================

        foreach (Transform spawnT in spawnTs)
        {
            if (spawnT == null || zookeeperPrefab == null)
            {
                Debug.LogError("Zookeeper spawn 場所がない!");
                continue;
            }
            var inst = Instantiate(zookeeperPrefab, spawnT);
            AILogicController aiLogic = inst.GetComponent<AILogicController>();
            aiLogic.SetStateByEnum(AILogicController.SelectedState.InfiniteChase);
        }
    }

    public void AddTime()
    {
        // ============ 追加: Zookeeperスポーン後は時間を追加しない ============
        if (zookeepersSpawned)
        {
            Debug.Log("Zookeeperがスポーン済みのため、時間は追加しない");
            return;
        }

        totalTime += addTimePerRoom;
    }
}