using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private Text[] txtRank = new Text[5];  // Rank text (1~5)
    private float[] times = new float[5];                   // best times
    private string[] labels = { "1st : ", "2nd : ", "3rd : ", "4th : ", "5th : " };

    void Start()
    {
        LoadRanking();
        DisplayRanking();
    }
    public void AddTime(float newTime)
    {
        for (int i = 0; i < times.Length; i++)
        {
            if (times[i] <= 0) times[i] = 9999f;
        }

        for (int i = 0; i < times.Length; i++)
        {
            if (newTime < times[i])
            {
                for (int j = times.Length - 1; j > i; j--)
                {
                    times[j] = times[j - 1];
                }
                times[i] = newTime;
                SaveRanking();
                DisplayRanking();
                return;
            }
        }
    }

    private void LoadRanking()
    {
        for (int i = 0; i < times.Length; i++)
        {
            times[i] = PlayerPrefs.GetFloat("BestTime" + i, 0f);
        }
    }

    private void SaveRanking()
    {
        for (int i = 0; i < times.Length; i++)
        {
            PlayerPrefs.SetFloat("BestTime" + i, times[i]);
        }
        PlayerPrefs.Save();
    }

    private void DisplayRanking()
    {
        for (int i = 0; i < txtRank.Length; i++)
        {
            if (txtRank[i] == null) continue;

            if (times[i] <= 0 || times[i] >= 9999f)
                txtRank[i].text = labels[i] + "--.--s";
            else
                txtRank[i].text = labels[i] + times[i].ToString("F2") + "s";
        }
    }

    public void ResetRanking()
    {
        for (int i = 0; i < times.Length; i++)
        {
            times[i] = 0f;
            PlayerPrefs.DeleteKey("BestTime" + i);
        }
        PlayerPrefs.Save();
        DisplayRanking();
    }
}
/*  confirm in GameManager
 * [SerializeField] private RankingManager rankingManager;

void GameFinish()
{
    float finishTime = elapsedTime;
    rankingManager.AddTime(finishTime);
}*/