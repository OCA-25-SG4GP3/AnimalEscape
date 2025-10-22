using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorPanelRoomTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private float totalTime = 120f; // total seconds, e.g., 2 minutes
    [SerializeField] private float addTimePerRoom = 30f; // total seconds add
    [SerializeField] private GameObject zookeeperPrefab;
    [SerializeField] private List<Transform> spawnTs = new();
    private string originalString = "Žc‚èŽžŠÔ : ";
    bool hasSpawnedOnce = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { Debug.Log("TIME END SET"); SetTimeEnd(); }
        if (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
            if (totalTime < 0) totalTime = 0;

            int minutes = Mathf.FloorToInt(totalTime / 60f);
            int seconds = Mathf.FloorToInt(totalTime % 60f);

            timeText.text = originalString + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else
        {
            if (!hasSpawnedOnce)
            {
                SetTimeEnd();
            }
        }
    }

    private void SetTimeEnd()
    {
        timeText.text = "Ž”ˆçˆõ‚ª—ˆ‚Ü‚·I";
        foreach (Transform spawnT in spawnTs)
        {
            if (spawnT == null || zookeeperPrefab == null)
            {
                Debug.LogError("missing!");
                continue;
            }
            var inst = Instantiate(zookeeperPrefab, spawnT);
            AILogicController aiLogic = inst.GetComponent<AILogicController>();
            aiLogic.SetStateByEnum(AILogicController.SelectedState.InfiniteChase);
        }
        hasSpawnedOnce = true;
    }

    public void AddTime()
    {
        totalTime += addTimePerRoom;
    }
}
